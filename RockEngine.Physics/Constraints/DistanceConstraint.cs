using OpenTK.Mathematics;

using RockEngine.Physics.Constraints;

namespace RockEngine.Physics
{
    public class DistanceConstraint : IConstraint
    {
        private readonly RigidBody _bodyA;
        private readonly RigidBody _bodyB;
        private readonly float _targetDistance;

        public DistanceConstraint(RigidBody a, RigidBody b, float distance)
        {
            _bodyA = a;
            _bodyB = b;
            _targetDistance = distance;
        }

        public void ApplyConstraint(float deltaTime)
        {
            // Calculate the vector between the bodies
            Vector3 delta = _bodyB.Position - _bodyA.Position;
            float currentDistance = delta.Length;
            Vector3 correction = delta * ((currentDistance - _targetDistance) / currentDistance);
            // Apply correction
            _bodyA.Position -= correction * 0.5f;
            _bodyB.Position += correction * 0.5f;
        }
    }
}