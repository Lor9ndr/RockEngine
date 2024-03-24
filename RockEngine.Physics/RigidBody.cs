using OpenTK.Mathematics;
using RockEngine.Physics.Colliders;

namespace RockEngine.Physics
{
    public class RigidBody
    {
        public Vector3 Position;
        public Quaternion Rotation = Quaternion.Identity;
        public ICollider Collider { get; private set;}
        public float Mass
        {
            get => _mass;
            set
            {
                _mass = value;
                UpdateMassDependencies();
            }
        }
        public bool IsStatic => _isStatic;
        public float InverseMass => _inverseMass;

        public Vector3 Velocity;
        public Vector3 AngularVelocity;

        public Matrix3 InertiaTensor { get; private set;}

        public Matrix3 InverseInertiaTensor { get; private set; }

        public float LinearDamping = 0.98f;
        public float AngularDamping = 0.95f;
        private float _inverseMass;
        private float _mass;
        private bool _isStatic;

        public RigidBody(Vector3 position, float mass, ICollider collider)
        {
            Position = position;
            InertiaTensor = Matrix3.Identity; // Placeholder, will be updated in UpdateMassDependencies
            InverseInertiaTensor = Matrix3.Identity; // Placeholder, will be updated in UpdateMassDependencies
            Collider = collider;
            Mass = mass; // Define mass last because it is calling UpdateMassDependencies(); 

        }

        private void UpdateMassDependencies()
        {
            _inverseMass = Mass != 0 ? 1 / Mass : 0;
            _isStatic = Mass == 0;

            // Use the collider's CalculateInertiaTensor method
            InertiaTensor = Collider.CalculateInertiaTensor(Mass);
            InverseInertiaTensor = InertiaTensor.Inverted();
        }

        public void ApplyForce(Vector3 force, Vector3 point)
        {
            Velocity += force * InverseMass;
            AngularVelocity += InverseInertiaTensor * Vector3.Cross(point - Position, force);
        }

        public void Simulate(float deltaTime, Vector3 gravity)
        {
            Collider.GetUpdatesFromBody(this);
            if(IsStatic)
            {
                return;
            }

            // RK4 integration for linear motion
            Vector3 initialPosition = Position;
            Vector3 initialVelocity = Velocity;

            // Calculate the four derivatives for position
            Vector3 k1Vel = deltaTime * initialVelocity;
            Vector3 k2Vel = deltaTime * (initialVelocity + 0.5f * k1Vel);
            Vector3 k3Vel = deltaTime * (initialVelocity + 0.5f * k2Vel);
            Vector3 k4Vel = deltaTime * (initialVelocity + k3Vel);

            // Update position based on the weighted sum of derivatives
            Position += (k1Vel + 2.0f * (k2Vel + k3Vel) + k4Vel) / 6.0f;

            // RK4 integration for velocity
            Vector3 acceleration = gravity; // This can be more complex if other forces are involved

            // Calculate the four derivatives for velocity
            Vector3 k1Acc = deltaTime * acceleration;
            Vector3 k2Acc = deltaTime * (acceleration + 0.5f * k1Acc);
            Vector3 k3Acc = deltaTime * (acceleration + 0.5f * k2Acc);
            Vector3 k4Acc = deltaTime * (acceleration + k3Acc);

            // Update velocity based on the weighted sum of derivatives
            Velocity += (k1Acc + 2.0f * (k2Acc + k3Acc) + k4Acc) / 6.0f;

            // Apply linear damping
            Velocity *= MathF.Pow(LinearDamping, deltaTime);

            // RK4 integration for angular motion
            Quaternion initialRotation = Rotation;
            Vector3 initialAngularVelocity = AngularVelocity;

            // Calculate the four derivatives for rotation (quaternion)
            Quaternion k1Rot = deltaTime * new Quaternion(initialAngularVelocity.X, initialAngularVelocity.Y, initialAngularVelocity.Z, 0) * initialRotation * 0.5f;
            Quaternion k2Rot = deltaTime * new Quaternion((initialAngularVelocity + 0.5f * k1Rot.Xyz).X, (initialAngularVelocity + 0.5f * k1Rot.Xyz).Y, (initialAngularVelocity + 0.5f * k1Rot.Xyz).Z, 0) * initialRotation * 0.5f;
            Quaternion k3Rot = deltaTime * new Quaternion((initialAngularVelocity + 0.5f * k2Rot.Xyz).X, (initialAngularVelocity + 0.5f * k2Rot.Xyz).Y, (initialAngularVelocity + 0.5f * k2Rot.Xyz).Z, 0) * initialRotation * 0.5f;
            Quaternion k4Rot = deltaTime * new Quaternion((initialAngularVelocity + k3Rot.Xyz).X, (initialAngularVelocity + k3Rot.Xyz).Y, (initialAngularVelocity + k3Rot.Xyz).Z, 0) * initialRotation * 0.5f;

            // Update rotation based on the weighted sum of derivatives
            Quaternion deltaRotation = (k1Rot + 2.0f * (k2Rot + k3Rot) + k4Rot) * (1.0f / 6.0f);
            Rotation += deltaRotation;
            Rotation.Normalize();

            // Apply angular damping
            AngularVelocity *= MathF.Pow(AngularDamping, deltaTime);
        }

        private void UpdateRotation(float deltaTime)
        {
            Quaternion w = new Quaternion(AngularVelocity.X, AngularVelocity.Y, AngularVelocity.Z, 0);
            Quaternion qDot = 0.5f * w * Rotation;
            Quaternion newRotation = Rotation + qDot * deltaTime;
            newRotation.Normalize();
            Rotation = newRotation;
        }
    }
}
