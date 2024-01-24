using OpenTK.Mathematics;

using RockEngine.Physics.Colliders;

namespace RockEngine.Physics
{
    public class RigidBody
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Velocity;
        public Vector3 CenterOfMass;
        public Vector3 AngularVelocity;
        public Vector3 CenterOfMassGlobal => Position + Vector3.Transform(CenterOfMass, Rotation);

        public MotionState MotionState;
        public float CoefficientOfFrictionDynamic { get; set; }  = 0.01f;
        public float CoefficientOfFrictionStatic { get; set; }  = 0.1f;
        public bool IsSleeping { get; private set; }
        public Collider Collider
        {
            get => _collider;
            set
            {
                _collider = value;
                _collider.Body = this;
                UpdateFields();
            }
        }

        internal Matrix3 InverseInertiaTensor;
        private Collider _collider;
        private float _inverseMass;
        private float _mass;
        private bool _isStatic;
        private Vector3 InertiaTensor;

        private const float SleepThreshold = 0.2f;
        private const float SleepTime = 1.0f; // Time in seconds
        private float _timeSinceLastMovement = 0.0f;

        public float InverseMass => _inverseMass;

        public bool IsStatic => _isStatic;
        public bool IsMovable => !_isStatic;

        public float Mass
        {
            get => _mass;
            set
            {
                _mass = value;
                UpdateFields();
            }
        }

        public Matrix3 InverseInertiaTensorWorld { get; private set; }

        private void UpdateFields()
        {
            bool isStatic = Mass == 0;
            if(isStatic)
            {
                _inverseMass = 0;
            }
            else
            {
                _inverseMass = 1 / _mass;
            }
            _isStatic = isStatic;
            CalculateInertiaTensor();
        }

        public RigidBody(Vector3 position, Vector3 velocity, float mass)
        {
            Position = position;
            Velocity = velocity;
            _mass = mass;
            Rotation = Quaternion.Identity;
            MotionState = new MotionState();
        }

        public void ApplyForce(Vector3 force)
        {
            if(IsStatic)
            {
                return; // No need to apply force to a static body
            }
            // Change in velocity is given by F = ma, or DeltaV = F/m
            Velocity += force * InverseMass;
        }

        public void ApplyTorque(Vector3 torque)
        {
            AngularVelocity += InverseInertiaTensorWorld * torque;
        }

        public void ApplyImpulse(Vector3 impulse)
        {
            // F = ma, so a = F/m. We can add this acceleration to the velocity.
            Velocity += impulse * _inverseMass;
        }
        public void ApplyImpulse(Vector3 impulse, Vector3 contactPoint)
        {
            if(IsStatic)
            {
                return; // No need to apply impulse to a static body
            }

            // Linear impulse application
            Velocity += InverseMass * impulse;

            // Convert the contact point to be relative to the center of mass
            Vector3 contactVector = contactPoint - CenterOfMassGlobal;

            // Angular impulse application
            // Calculate the torque induced by the impulse relative to the center of mass
            Vector3 torque = Vector3.Cross(contactVector, impulse);

            // Apply the torque to the angular velocity, taking into account the body's inertia tensor
            ApplyTorque(torque);
        }

        public void PutToSleep()
        {
            IsSleeping = true;
            Velocity = Vector3.Zero;
            AngularVelocity = Vector3.Zero;
        }

        public void WakeUp()
        {
            IsSleeping = false;
        }

        public void Update(float deltaTime, Vector3 gravity)
        {
            if(Velocity.LengthSquared < SleepThreshold * SleepThreshold &&
            AngularVelocity.LengthSquared < SleepThreshold * SleepThreshold)
            {
                _timeSinceLastMovement += deltaTime;
                if(_timeSinceLastMovement >= SleepTime)
                {
                    PutToSleep();
                }
            }
            else
            {
                _timeSinceLastMovement = 0.0f;
                WakeUp();
            }
            if(IsSleeping || !IsMovable)
            {
                return;
            }
            //deltaTime = CalculateTimeStep(deltaTime);

            // Runge-Kutta 4th order method
            Vector3 k1 = deltaTime * Velocity;
            Vector3 k2 = deltaTime * (Velocity + 0.5f * k1);
            Vector3 k3 = deltaTime * (Velocity + 0.5f * k2);
            Vector3 k4 = deltaTime * (Velocity + k3);
            Position += (k1 + 2f * (k2 + k3) + k4) / 6f;

            // Update linear velocity with gravity and damping
            Velocity += gravity * deltaTime; // Update velocity with acceleration

            // Quadratic damping
            Velocity += 0.5f * CoefficientOfFrictionDynamic * Velocity * Velocity.Length * deltaTime;

            // Rotational friction
            AngularVelocity -= 0.5f * CoefficientOfFrictionDynamic * AngularVelocity * AngularVelocity.Length * deltaTime;

            // Update the world-space inverse inertia tensor
            Matrix3 rotationMatrix = Matrix3.CreateFromQuaternion(Rotation);
            InverseInertiaTensorWorld = rotationMatrix * InverseInertiaTensor * Matrix3.Transpose(rotationMatrix);

            // Quaternion-based rotation update
            Quaternion angularVelocityQuaternion = new Quaternion(AngularVelocity.X, AngularVelocity.Y, AngularVelocity.Z, 0);
            Quaternion spin = 0.5f * Quaternion.Multiply(angularVelocityQuaternion, Rotation);
            Quaternion newRotation = new Quaternion(Rotation.X + spin.X * deltaTime,
                                                    Rotation.Y + spin.Y * deltaTime,
                                                    Rotation.Z + spin.Z * deltaTime,
                                                    Rotation.W + spin.W * deltaTime);
            Rotation = Quaternion.Normalize(newRotation);
        }

        private void CalculateInertiaTensor()
        {
            InertiaTensor = Vector3.One;
            InverseInertiaTensor = Matrix3.Identity;

            if(IsStatic)
            {
                return;
            }

            InertiaTensor = Collider.GetLocalInertiaTensor(Mass);
            InverseInertiaTensor.Diagonal = new Vector3(1.0f / InertiaTensor.X, 1.0f / InertiaTensor.Y, 1.0f / InertiaTensor.Z);
        }
    }
}
