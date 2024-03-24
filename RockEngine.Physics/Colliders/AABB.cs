using OpenTK.Mathematics;
using RockEngine.Physics.CollisionResolution;

namespace RockEngine.Physics.Colliders
{
    public struct AABB : ICollider
    {
        public Vector3 Max { get; private set; }
        public Vector3 Min { get; private set; }
        public Vector3 Center { get; private set; }
        public bool WasCollided { get; set; }
        public float Restitution { get; set; } = 0.5f;

        // Constructor that takes the center position and extents of the AABB
        public AABB(Vector3 center, Vector3 extents)
        {
            Center = center;
            Min = -extents;
            Max = extents;
        }

        // Method to update the AABB based on a new center position
        public void GetUpdatesFromBody(RigidBody body)
        {
            Center = body.Position;
        }

        // Method to expand the AABB to include another AABB
        public void Encapsulate(AABB other)
        {
            Min = Vector3.ComponentMin(Min, other.Min);
            Max = Vector3.ComponentMax(Max, other.Max);
        }

        public bool Visit(AABB other, out CollisionManifold manifold)
        {
            // Initialize the manifold
            manifold = new CollisionManifold
            {
                Colliding = false,
                Normal = Vector3.Zero,
                Depth = 0f,
                ContactPoints = new List<Vector3>()
            };
            // Transforming max and min to global values
            var globalMax = Center + Max;
            var globalMin = Center + Min;

            var otherGlobalMax = other.Center + other.Max;
            var otherGlobalMin = other.Center + other.Min;

            // Calculate overlap on each axis
            float overlapX = Math.Min(globalMax.X - otherGlobalMin.X, otherGlobalMax.X - globalMin.X);
            float overlapY = Math.Min(globalMax.Y - otherGlobalMin.Y, otherGlobalMax.Y - globalMin.Y);
            float overlapZ = Math.Min(globalMax.Z - otherGlobalMin.Z, otherGlobalMax.Z - globalMin.Z);
            if(overlapX > 0 && overlapY > 0 && overlapZ > 0)
            {
                // Collision has occurred
                manifold.Colliding = true;

                // Determine the smallest axis of overlap, which will be the collision normal
                if(overlapX < overlapY && overlapX < overlapZ)
                {
                    manifold.Normal = (globalMax.X - otherGlobalMin.X < otherGlobalMax.X - globalMin.X) ? new Vector3(-1, 0, 0) : new Vector3(1, 0, 0);
                    manifold.Depth = overlapX;
                }
                else if(overlapY < overlapZ)
                {
                    manifold.Normal = (globalMax.Y - otherGlobalMin.Y < otherGlobalMax.Y - globalMin.Y) ? new Vector3(0, -1, 0) : new Vector3(0, 1, 0);
                    manifold.Depth = overlapY;
                }
                else
                {
                    manifold.Normal = (globalMax.Z - otherGlobalMin.Z < otherGlobalMax.Z - globalMin.Z) ? new Vector3(0, 0, -1) : new Vector3(0, 0, 1);
                    manifold.Depth = overlapZ;
                }
                Vector3 overlapMin = Vector3.ComponentMax(globalMax, otherGlobalMin);
                Vector3 overlapMax = Vector3.ComponentMin(globalMin, otherGlobalMax);

                // Calculate the corners of the overlapping region
                List<Vector3> contactPoints = new List<Vector3>
                {
                    new Vector3(overlapMin.X, overlapMin.Y, overlapMin.Z),
                    new Vector3(overlapMin.X, overlapMin.Y, overlapMax.Z),
                    new Vector3(overlapMin.X, overlapMax.Y, overlapMin.Z),
                    new Vector3(overlapMin.X, overlapMax.Y, overlapMax.Z),
                    new Vector3(overlapMax.X, overlapMin.Y, overlapMin.Z),
                    new Vector3(overlapMax.X, overlapMin.Y, overlapMax.Z),
                    new Vector3(overlapMax.X, overlapMax.Y, overlapMin.Z),
                    new Vector3(overlapMax.X, overlapMax.Y, overlapMax.Z)
                };
                // Filter out the points that are not within the overlapping region on all axes
                manifold.ContactPoints = contactPoints.FindAll(point =>
                    point.X >= overlapMin.X && point.X <= overlapMax.X &&
                    point.Y >= overlapMin.Y && point.Y <= overlapMax.Y &&
                    point.Z >= overlapMin.Z && point.Z <= overlapMax.Z);
            }
            return manifold.Colliding;
        }

        public readonly AABB GetAABB() => this;

        public readonly bool Accept(ICollider visitor, out CollisionManifold manifold)
        {
            return visitor.Visit(this, out manifold);
        }

        public Matrix3 CalculateInertiaTensor(float mass)
        {
            Vector3 size = Max - Min;
            float width = size.X;
            float height = size.Y;
            float depth = size.Z;

            float ix = (1 / 12.0f) * mass * (height * height + depth * depth);
            float iy = (1 / 12.0f) * mass * (width * width + depth * depth);
            float iz = (1 / 12.0f) * mass * (width * width + height * height);

            return new Matrix3(new Vector3(ix, 0, 0), new Vector3(0, iy, 0), new Vector3(0, 0, iz));
        }

        public bool Visit(OBB other, out CollisionManifold manifold)
        {
            throw new NotImplementedException();
        }
    }
}