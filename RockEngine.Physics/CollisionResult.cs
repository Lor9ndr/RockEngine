using OpenTK.Mathematics;

namespace RockEngine.Physics
{
    public ref struct CollisionResult
    {
        public bool IsCollided;
        public Vector3[] ContactPoints;
        public Vector3 Normal;
        public float PenetrationDepth;

        public CollisionResult(bool isCollided, Vector3[] collisionPoints, Vector3 normal,  float penetrationDepth)
        {
             IsCollided  = isCollided;
            ContactPoints = collisionPoints;
            Normal = normal;
            PenetrationDepth = penetrationDepth;
        }

        public CollisionResult() { }
    }
}
