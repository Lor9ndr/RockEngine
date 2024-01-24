using OpenTK.Mathematics;

using RockEngine.Common.Vertices;

using System.Buffers;

namespace RockEngine.Physics.Colliders
{
    /// <summary>
    /// OBB
    /// </summary>
    public class BoxCollider : Collider
    {
        public Vector3 Min;
        public Vector3 Max;
        public Vector3 MinGlobal => Vector3.Transform(Min, Body.Rotation) + Body.Position;
        public Vector3 MaxGlobal => Vector3.Transform(Max, Body.Rotation) + Body.Position;
        public Vector3 Extents => (Max - Min) / 2.0f;

        private static readonly Vector3[] LocalPoints =
        [
            new Vector3(-1, -1, -1),
            new Vector3(-1, -1, 1),
            new Vector3(-1, 1, -1),
            new Vector3(-1, 1, 1),
            new Vector3(1, -1, -1),
            new Vector3(1, -1, 1),
            new Vector3(1, 1, -1),
            new Vector3(1, 1, 1)
         ];

        public BoxCollider(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }

        public override CollisionResult CheckCollision(BoxCollider otherCollider)
        {
            Span<Vector3> axes = stackalloc Vector3[15];

            // Get the axes of the box
            Span<Vector3> boxAxes =
            [
                Vector3.Transform(Vector3.UnitX, Body.Rotation),
                Vector3.Transform(Vector3.UnitY, Body.Rotation),
                Vector3.Transform(Vector3.UnitZ, Body.Rotation),
            ];

            // Get the axes of the other box
            Span<Vector3> otherBoxAxes =
            [
                Vector3.Transform(Vector3.UnitX, otherCollider.Body.Rotation),
                Vector3.Transform(Vector3.UnitY, otherCollider.Body.Rotation),
                Vector3.Transform(Vector3.UnitZ, otherCollider.Body.Rotation),
            ];

            // Add the axes to the list
            axes[0] = boxAxes[0];
            axes[1] = boxAxes[1];
            axes[2] = boxAxes[2];
            axes[3] = otherBoxAxes[0];
            axes[4] = otherBoxAxes[1];
            axes[5] = otherBoxAxes[2];

            int index = 6;
            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    Vector3 crossProduct = Vector3.Cross(boxAxes[i], otherBoxAxes[j]);

                    // Check if the cross product is the zero vector
                    if(crossProduct.LengthSquared > float.E)
                    {
                        axes[index++] = crossProduct;
                    }
                }
            }

            float minOverlap = float.MaxValue;
            Vector3 minOverlapAxis = Vector3.Zero;

            for(int i = 0; i < index; i++)
            {
                Vector3 axis = axes[i];
                var overlap = IsOverlapOnAxis(this, otherCollider, axis);
                if(overlap == 0)
                {
                    return new CollisionResult();
                }
                else if(Math.Abs(overlap) < Math.Abs(minOverlap))
                {
                    minOverlap = overlap;
                    minOverlapAxis = axis;
                }
            }

            Vector3 normal = minOverlap < 0 ? -minOverlapAxis : minOverlapAxis;

            // calculate penetration depth as the smallest overlap
            float penetrationDepth = Math.Abs(minOverlap);

            // calculate collision point as the closest points between the two boxes along
            var collisionPoints = CalculateContactPoints(this, otherCollider);

            return new CollisionResult(true, collisionPoints, normal, penetrationDepth);
        }

        private float IsOverlapOnAxis(BoxCollider box1, BoxCollider box2, Vector3 axis)
        {
            // Project the two boxes onto the axis
            float box1Min = float.MaxValue, box1Max = float.MinValue;
            float box2Min = float.MaxValue, box2Max = float.MinValue;

            foreach(Vector3 point in box1.GetPoints())
            {
                float val = Vector3.Dot(point, axis);
                box1Min = float.Min(box1Min, val);
                box1Max = float.Max(box1Max, val);
            }

            foreach(Vector3 point in box2.GetPoints())
            {
                float val = Vector3.Dot(point, axis);
                box2Min = float.Min(box2Min, val);
                box2Max = float.Max(box2Max, val);
            }

            // Calculate the overlap
            float overlap = float.Min(box1Max, box2Max) - float.Max(box1Min, box2Min);

            // Return 0 if there is no overlap
            if(overlap < 0)
            {
                return 0;
            }

            // Return the overlap
            return overlap; // Always return a positive overlap
        }

        private Vector3[] CalculateContactPoints(BoxCollider box1, BoxCollider box2)
        {
            List<Vector3> contactPoints = new List<Vector3>();
            foreach(Vector3 vertex in box1.GetPoints())
            {
                if(IsPointInsideBox(vertex, box2) ||
                    IsPointInsideBox(box2.MinGlobal, box1) ||
                    IsPointInsideBox(box2.MaxGlobal, box1))
                {
                    contactPoints.Add(vertex);
                }
            }
            foreach(Vector3 vertex in box2.GetPoints())
            {
                if(IsPointInsideBox(vertex, box1) ||
                    IsPointInsideBox(box1.MinGlobal, box2) ||
                    IsPointInsideBox(box1.MaxGlobal, box2))
                {
                    contactPoints.Add(vertex);
                }
            }

            return contactPoints.ToArray();
        }

        private bool IsValidContactPoint(Vector3 point, BoxCollider box1, BoxCollider box2)
        {
            // Check if the point is within the bounds of both boxes
            return IsPointInsideBox(point, box1) && IsPointInsideBox(point, box2);
        }

        private bool IsPointInsideBox(Vector3 point, BoxCollider box)
        {
            // Transform the point to the box's local space
            Vector3 localPoint = Vector3.Transform(point - box.Body.Position, box.Body.Rotation);

            // Check if the point is within the box's bounds
            return localPoint.X >= box.Min.X && localPoint.X <= box.Max.X &&
                   localPoint.Y >= box.Min.Y && localPoint.Y <= box.Max.Y &&
                   localPoint.Z >= box.Min.Z && localPoint.Z <= box.Max.Z;
        }

        private Vector3[] GetPoints()
        {
            Vector3[] points = new Vector3[8];
            for(int i = 0; i < 8; i++)
            {
                points[i] = Vector3.Transform(LocalPoints[i] * Extents, Body.Rotation) + Body.Position;
            }
            return points;
        }

        public override CollisionResult CheckCollision(SphereCollider otherCollider)
            => otherCollider.CheckCollision(this);

        public override ref Vector3[] GetVertices()
        {
            return ref BoxVertices;
        }

        public override Vector3 GetLocalInertiaTensor(float mass)
        {
            // Assuming the object is a box with dimensions x, y, z
            float x = Extents.X * 2;
            float y = Extents.Y * 2;
            float z = Extents.Z * 2;

            // Calculate the inertia tensor for a box
            float Ixx = (1.0f / 12.0f) * mass * (y * y + z * z);
            float Iyy = (1.0f / 12.0f) * mass * (x * x + z * z);
            float Izz = (1.0f / 12.0f) * mass * (x * x + y * y);

            return new Vector3(Ixx, Iyy, Izz);
        }

        private static Vector3[] BoxVertices = Vertex3D.CubeVertices.Select(s => s.Position).ToArray();
    }
}

