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
        private readonly ArrayPool<Vector3> axisPool = ArrayPool<Vector3>.Shared;

        public Vector3 Min;
        public Vector3 Max;
        public Vector3 MinGlobal => Vector3.Transform(Min, Body.Rotation) + Body.Position;
        public Vector3 MaxGlobal => Vector3.Transform(Max, Body.Rotation) + Body.Position;
        public Vector3 Extents => (Max - Min) / 2.0f;

        public BoxCollider(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }

        public override CollisionResult CheckCollision(BoxCollider otherCollider)
        {
            Vector3[ ] axes = axisPool.Rent(15);

            // Get the axes of the box
            Vector3[ ] boxAxes = axisPool.Rent(3);
            boxAxes[0] = Vector3.Transform(Vector3.UnitX, Body.Rotation);
            boxAxes[1] = Vector3.Transform(Vector3.UnitY, Body.Rotation);
            boxAxes[2] = Vector3.Transform(Vector3.UnitZ, Body.Rotation);

            // Get the axes of the other box
            Vector3[ ] otherBoxAxes = axisPool.Rent(3);
            otherBoxAxes[0] = Vector3.Transform(Vector3.UnitX, otherCollider.Body.Rotation);
            otherBoxAxes[1] = Vector3.Transform(Vector3.UnitY, otherCollider.Body.Rotation);
            otherBoxAxes[2] = Vector3.Transform(Vector3.UnitZ, otherCollider.Body.Rotation);

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
                    if(crossProduct.LengthSquared > 0.0001f) // Use a small threshold instead of float.Epsilon
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
                    axisPool.Return(axes);
                    axisPool.Return(boxAxes);
                    axisPool.Return(otherBoxAxes);
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

            axisPool.Return(axes);
            axisPool.Return(boxAxes);
            axisPool.Return(otherBoxAxes);
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
                box1Min = Math.Min(box1Min, val);
                box1Max = Math.Max(box1Max, val);
            }

            foreach(Vector3 point in box2.GetPoints())
            {
                float val = Vector3.Dot(point, axis);
                box2Min = Math.Min(box2Min, val);
                box2Max = Math.Max(box2Max, val);
            }

            // Calculate the overlap
            float overlap = Math.Min(box1Max, box2Max) - Math.Max(box1Min, box2Min);

            // Return 0 if there is no overlap
            if(overlap < 0)
            {
                return 0;
            }

            // Return the overlap
            return Math.Abs(overlap); // Always return a positive overlap
        }

        private Vector3[] CalculateContactPoints(BoxCollider box1, BoxCollider box2)
        {
            List<Vector3> contactPoints = new List<Vector3>();
            foreach(Vector3 vertex in box1.GetPoints())
            {
                if(IsPointInsideBox(vertex, box2))
                {
                    contactPoints.Add(vertex);
                }
            }

            // Clip the vertices of box2 against the faces of box1
            foreach(Vector3 vertex in box2.GetPoints())
            {
                if(IsPointInsideBox(vertex, box1))
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
            Vector3[] localPoints =
            [
                    new Vector3(-Extents.X, -Extents.Y, -Extents.Z),
                    new Vector3(-Extents.X, -Extents.Y, Extents.Z),
                    new Vector3(-Extents.X, Extents.Y, -Extents.Z),
                    new Vector3(-Extents.X, Extents.Y, Extents.Z),
                    new Vector3(Extents.X, -Extents.Y, -Extents.Z),
                    new Vector3(Extents.X, -Extents.Y, Extents.Z),
                    new Vector3(Extents.X, Extents.Y, -Extents.Z),
                    new Vector3(Extents.X, Extents.Y, Extents.Z)
            ];

            for(int i = 0; i < 8; i++)
            {
                points[i] = Vector3.Transform(localPoints[i], Body.Rotation) + Body.Position;
            }

            return points;
        }

        public override CollisionResult CheckCollision(SphereCollider otherCollider)
            => otherCollider.CheckCollision(this);

        public override ref Vector3[ ] GetVertices()
        {
            return ref BoxVertices;
        }
        public static Vector3[ ] BoxVertices = Vertex3D.CubeVertices.Select(s => s.Position).ToArray();
    }
}

