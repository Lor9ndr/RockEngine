using OpenTK.Mathematics;

namespace RockEngine.Physics
{
    public ref struct CollisionResult
    {
        public bool IsCollided;
        public Vector3 ContactPoint;
        public Vector3 Normal;
        public float PenetrationDepth;

        public CollisionResult(bool isCollided, Vector3 collisionPoint, Vector3 normal,  float penetrationDepth)
        {
             IsCollided  = isCollided;
            ContactPoint = collisionPoint;
            Normal = normal;
            PenetrationDepth = penetrationDepth;
        }

        public CollisionResult() { }
    }
}
