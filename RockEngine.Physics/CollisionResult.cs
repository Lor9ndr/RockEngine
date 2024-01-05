using OpenTK.Mathematics;

namespace RockEngine.Physics
{
    public struct CollisionResult
    {
        public Vector3 ContactPoint;
        public Vector3 Normal;
        public float PenetrationDepth;
        public float ImpulseMagnitude;

        public CollisionResult(Vector3 collisionPoint, Vector3 normal, float penetrationDepth)
        {
            ContactPoint = collisionPoint;
            Normal = normal;
            PenetrationDepth = penetrationDepth;
        }

        public CollisionResult() { }
    }
}
