using OpenTK.Mathematics;

namespace RockEngine.Physics.CollisionResolution
{
    // Structure to hold collision details
    public ref struct CollisionManifold
    {
        public bool Colliding;
        public Vector3 Normal;
        public float Depth;
        public List<Vector3> ContactPoints;
    }
}
