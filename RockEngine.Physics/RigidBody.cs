using OpenTK.Mathematics;

using RockEngine.Common.Editor;
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
        public float FrictionCoefficient = 0.99f;
        public MotionState MotionState;

        private Vector3 _totalForce;
        internal Vector3 InverseInertiaTensor;
        private Collider? _collider;
        private Vector3 _totalTorque;
        private float _inverseMass;
        [UI]
        private float _mass;
        private bool _isStatic;

        public Collider? Collider
        {
            get => _collider;
            set
            {
                _collider = value;
                CalculateInertiaTensor();
            }
        }

        public float InverseMass => _inverseMass;

        public bool IsStatic => _isStatic;
        public bool IsMovable =>  !_isStatic;

        public float Mass
        {
            get => _mass;
            set
            {
                _mass = value;
                UpdateFields();
            }
        }

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

        private const float sleepThreshold = 0.01f; // Пороговое значение скорости для сна
        private const float wakeThreshold = 0.1f; // Пороговое значение скорости для пробуждения

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
            ApplyForce(force, CenterOfMass);
        }

        public void ApplyTorque(Vector3 force)
        {
            if(IsStatic)
            {
                return; // No need to apply torque to a static body
            }
            _totalTorque += force;
        }

        public void ApplyImpulse(Vector3 impulse)
        {
            ApplyImpulse(impulse, CenterOfMass);
        }

        public void ApplyForce(Vector3 force, Vector3 point)
        {
            if(IsStatic)
            {
                return; // No need to apply force to a static body
            }
            _totalForce += force;
            // Calculate the torque and add it to the total torque
            Vector3 torque = Vector3.Cross(point, force);
            _totalTorque += torque;
        }

        public void ApplyImpulse(Vector3 impulse, Vector3 point)
        {
            if(IsStatic)
            {
                return; // No need to apply impulse to a static body
            }

            // Apply the impulse to the velocity
            _totalForce += impulse * InverseMass;

            // Calculate the angular impulse and apply it to the angular velocity
            Vector3 angularImpulse = Vector3.Cross(point, impulse);
            AngularVelocity += InverseInertiaTensor * angularImpulse;
        }

        private void CalculateInertiaTensor()
        {
            if(IsStatic)
            {
                return;
            }
            if(Collider is BoxCollider boxCollider)
            {
                // Assuming the object is a box with dimensions x, y, z
                float x = boxCollider.Extents.X;
                float y = boxCollider.Extents.Y;
                float z = boxCollider.Extents.Z;

                // Calculate the inertia tensor for a box
                float ix = (1f / 12f) * Mass * (y * y + z * z);
                float iy = (1f / 12f) * Mass * (x * x + z * z);
                float iz = (1f / 12f) * Mass * (x * x + y * y);

                // The inverse inertia tensor is the inverse of the inertia tensor
                InverseInertiaTensor = new Vector3(1 / ix, 1 / iy, 1 / iz);
            }
            else if(Collider is SphereCollider sphereCollider)
            {
                // Assuming the object is a sphere with radius r
                float r = sphereCollider.Radius;

                // Calculate the inertia tensor for a sphere
                float i = (2f / 5f) * Mass * r * r;

                // The inverse inertia tensor is the inverse of the inertia tensor
                InverseInertiaTensor = new Vector3(1 / i, 1 / i, 1 / i);
            }
        }

        public void Update(float deltaTime, Vector3 gravity)
        {
            if(!IsMovable)
            {
                return;
            }

            // Apply gravity
            _totalForce += Mass * gravity;

            // Update linear velocity
            Velocity += _totalForce * InverseMass * deltaTime;
            Velocity *= FrictionCoefficient;
            // Update position
            Position += Velocity * deltaTime;

            // Update angular velocity
            AngularVelocity += InverseInertiaTensor * _totalTorque * deltaTime;
            AngularVelocity *= FrictionCoefficient;

            // Update rotation
            Quaternion angularVelocityQuaternion = new Quaternion(AngularVelocity.X, AngularVelocity.Y, AngularVelocity.Z, 0);
            Quaternion deltaRotation = angularVelocityQuaternion * Rotation *  (deltaTime * 0.5f);
            Rotation += deltaRotation;
            Rotation.Normalize();

            // Reset total force and torque for the next frame
            _totalForce = Vector3.Zero;
            _totalTorque = Vector3.Zero;
        }
    }
}
