using OpenTK.Mathematics;

namespace RockEngine.Physics
{
    public class RigidBody
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Velocity;
        public float Mass;
        public Collider? Collider;
        public Vector3 TotalForce;
        public Vector3 AngularVelocity;
        public float FrictionCoefficient = 0.3f;

        public bool IsStatic => Mass == 0.0f;
        public bool IsMovable => !IsStatic;

        public RigidBody(Vector3 position, Vector3 velocity, float mass)
        {
            Position = position;
            Velocity = velocity;
            Mass = mass;
            Rotation = Quaternion.Identity;
        }

        public void ApplyForce(Vector3 force)
        {
            TotalForce += force;
        }

        public void ApplyTorque(Vector3 torque)
        {
            AngularVelocity += torque / Mass;
        }

        public void Update(float deltaTime, Vector3 gravity)
        {
            if(Mass > 0)
            {
                // Apply gravity force
                ApplyForce(gravity * Mass);

                // Calculate the normal force (assuming the object is on a flat surface)
                Vector3 normalForce = -gravity * Mass;

                // Calculate the maximum friction force based on the coefficient of friction
                float maxFrictionForceMagnitude = FrictionCoefficient * normalForce.Length;

                // Calculate the friction force direction
                Vector3 frictionDirection = Velocity.Length == 0 ? Velocity : -Velocity.Normalized(); // Opposes the direction of motion

                // Calculate the friction force magnitude
                float frictionForceMagnitude = Math.Min(maxFrictionForceMagnitude, TotalForce.Length); // Limit the friction force

                // Calculate the friction force vector
                Vector3 frictionForce = frictionDirection * frictionForceMagnitude;

                // Apply the friction force
                ApplyForce(frictionForce);

                // Update the velocity based on the total force applied
                Vector3 acceleration = TotalForce / Mass;
                Velocity += acceleration * deltaTime;
             
                // Update the position based on the new velocity
                Position += Velocity * deltaTime;

                // Calculate the angular acceleration based on the torque applied
                Vector3 angularAcceleration = AngularVelocity / deltaTime;

                // Update the angular velocity based on the angular acceleration
                AngularVelocity += angularAcceleration * deltaTime;

                // Calculate the rotation quaternion based on the angular velocity
                Quaternion deltaRotation = new Quaternion(AngularVelocity * deltaTime, 1); // Assuming the w component is 0 for angular velocity
                Rotation = Quaternion.Normalize(Rotation * deltaRotation); // Apply the new rotation after the previous rotation

                // Reset the total force for the next simulation step
                TotalForce = Vector3.Zero;
                
            }
        }
    }
}
