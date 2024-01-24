using RockEngine.Physics.Colliders;

namespace RockEngine.Physics
{
    public class BVHNode
    {
        public BoxCollider BoundingBox { get; set; }
        public RigidBody Body { get; set; }
        public BVHNode Left { get; set; }
        public BVHNode Right { get; set; }

        public static BVHNode Build(List<RigidBody> bodies)
        {
            // Implement your BVH building algorithm here.
            // This could be a simple top-down approach (like a binary space partitioning tree),
            // or a more complex bottom-up approach (like a surface area heuristic).
            // The goal is to minimize the total volume of all bounding boxes in the tree,
            // which will reduce the number of unnecessary collision checks.
            return  null;
        }
    }
}
