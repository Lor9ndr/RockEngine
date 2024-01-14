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
        public float CoefficientOfFrictionDynamic { get; set; }  = 0.001f;
        public float CoefficientOfFrictionStatic { get; set; }  = 0.1f;
        public Collider Collider
        {
            get => _collider;
            set
            {
                _collider = value;
                _collider.Body = this;
                CalculateInertiaTensor();
            }
        }

        internal Matrix3 InverseInertiaTensor;
        private Collider _collider;
        private float _inverseMass;
        private float _mass;
        private bool _isStatic;
        private Matrix3 InertiaTensor;

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
            Mass = mass;
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
            AngularVelocity = InverseInertiaTensorWorld * torque;
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
            AngularVelocity += InverseInertiaTensorWorld * torque;
        }

        public void Update(float deltaTime, Vector3 gravity)
        {
            if(!IsMovable)
            {
                return;
            }

            // Update linear velocity with gravity and damping
            Velocity += gravity * deltaTime;
            Velocity *= 1 - CoefficientOfFrictionDynamic; // Apply friction to linear velocity

            // Update position with velocity
            Position += Velocity * deltaTime;

            // Update angular velocity with damping
            AngularVelocity *= 1 - CoefficientOfFrictionDynamic; // Apply friction to angular velocity

            // Update the world-space inverse inertia tensor
            Matrix3 rotationMatrix = Matrix3.CreateFromQuaternion(Rotation);
            InverseInertiaTensorWorld = rotationMatrix * InverseInertiaTensor * Matrix3.Transpose(rotationMatrix);

            // Integrate angular velocity to update rotation
            Quaternion angularVelocityQuaternion = new Quaternion(AngularVelocity.X, AngularVelocity.Y, AngularVelocity.Z, 0);
            Quaternion spin = Quaternion.Multiply(angularVelocityQuaternion, Rotation) * 0.5f;

            // Integrate rotation using the spin
            Rotation += spin * deltaTime;
            Rotation = Quaternion.Normalize(Rotation);
        }

        private void CalculateInertiaTensor()
        {
            if(IsStatic)
            {
                return;
            }
            InverseInertiaTensor = Matrix3.Identity;
            if(Collider is BoxCollider boxCollider)
            {
                // Assuming the object is a box with dimensions x, y, z
                float x = boxCollider.Extents.X * 2;
                float y = boxCollider.Extents.Y * 2;
                float z = boxCollider.Extents.Z * 2;

                // Calculate the inertia tensor for a box
                float Ixx = (1.0f / 12.0f) * Mass * (y * y + z * z);
                float Iyy = (1.0f / 12.0f) * Mass * (x * x + z * z);
                float Izz = (1.0f / 12.0f) * Mass * (x * x + y * y);

                // Calculate the inverse inertia tensor
                InverseInertiaTensor = new Matrix3(
                    1.0f / Ixx, 0, 0,
                    0, 1.0f / Iyy, 0,
                    0, 0, 1.0f / Izz
                );
                InertiaTensor = Matrix3.Identity;
                InertiaTensor.M11 = Ixx;
                InertiaTensor.M22 = Iyy;
                InertiaTensor.M33 = Izz;
            }
            else if(Collider is SphereCollider sphereCollider)
            {
                // Assuming the object is a sphere with radius r
                float r = sphereCollider.Radius;

                // Calculate the inertia tensor for a sphere
                float i = 2f / 5f * Mass * r * r;

                // The inverse inertia tensor is the inverse of the inertia tensor
                InverseInertiaTensor = Matrix3.Identity;
                InverseInertiaTensor.M11 = 1 / i;
                InverseInertiaTensor.M22 = 1 / i;
                InverseInertiaTensor.M33 = 1 / i;

                InertiaTensor = Matrix3.Identity;
                InertiaTensor.M11 = i;
                InertiaTensor.M22 = i;
                InertiaTensor.M33 = i;
            }
        }
    }
}
