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
            boxAxes[0] = Vector3.Transform(Vector3.UnitX, this.Body.Rotation);
            boxAxes[1] = Vector3.Transform(Vector3.UnitY, this.Body.Rotation);
            boxAxes[2] = Vector3.Transform(Vector3.UnitZ, this.Body.Rotation);

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
            Vector3 collisionPoint = CalculateContactPoint(this, otherCollider);

            axisPool.Return(axes);
            axisPool.Return(boxAxes);
            axisPool.Return(otherBoxAxes);
            return new CollisionResult(true, collisionPoint, normal, penetrationDepth);
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
            return box1Max > box2Max ? overlap : -overlap;
        }
        private Vector3 CalculateContactPoint(BoxCollider box1, BoxCollider box2)
        {
            // Transform the local min and max points to global space
            Vector3 box1MinGlobal = Vector3.Transform(box1.Min, box1.Body.Rotation) + box1.Body.Position;
            Vector3 box1MaxGlobal = Vector3.Transform(box1.Max, box1.Body.Rotation) + box1.Body.Position;
            Vector3 box2MinGlobal = Vector3.Transform(box2.Min, box2.Body.Rotation) + box2.Body.Position;
            Vector3 box2MaxGlobal = Vector3.Transform(box2.Max, box2.Body.Rotation) + box2.Body.Position;

            // Calculate the overlapping min and max points along each axis in global space
            Vector3 overlapMin = Vector3.ComponentMax(box1MinGlobal, box2MinGlobal);
            Vector3 overlapMax = Vector3.ComponentMin(box1MaxGlobal, box2MaxGlobal);

            // Calculate the contact point as the center of the overlapping volume in global space
            Vector3 contactPoint = (overlapMin + overlapMax) / 2.0f;

            return contactPoint;
        }

        private Vector3 CalculateNormal(BoxCollider box1, BoxCollider box2)
        {
            // Calculate the difference between the centers of the two boxes
            Vector3 diff = box2.Body.Position - box1.Body.Position;

            // Normalize the difference to get the collision normal
            Vector3 normal = Vector3.Normalize(diff);

            return normal;
        }

        private Vector3[ ] GetPoints()
        {

            Vector3[ ] points = new Vector3[8];
            Vector3[ ] localPoints =
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

/*        private float CalculatePenetrationDepth(BoxCollider box1, BoxCollider box2, Vector3 collisionNormal)
        {
            float box1Min = float.MaxValue, box1Max = float.MinValue;
            float box2Min = float.MaxValue, box2Max = float.MinValue;

            foreach(Vector3 point in box1.GetPoints())
            {
                float val = Vector3.Dot(point, collisionNormal);
                box1Min = Math.Min(box1Min, val);
                box1Max = Math.Max(box1Max, val);
            }

            foreach(Vector3 point in box2.GetPoints())
            {
                float val = Vector3.Dot(point, collisionNormal);
                box2Min = Math.Min(box2Min, val);
                box2Max = Math.Max(box2Max, val);
            }

            // Calculate the overlap distance
            float overlap = Math.Min(box1Min, box2Min) - Math.Max(box1Min, box2Min);

            // Return the absolute value of the overlap as the penetration depth
            return Math.Abs(overlap);
        }

        private Vector3 CalculateContactPoint(BoxCollider box1, BoxCollider box2, Vector3 collisionNormal)
        {
            // Find the contact point on box1
            Vector3 contactPoint1 = Vector3.Zero;
            float minProjection1 = float.MaxValue;

            foreach(Vector3 point in box1.GetPoints())
            {
                float projection = Vector3.Dot(point, -collisionNormal);
                if(projection < minProjection1)
                {
                    minProjection1 = projection;
                    contactPoint1 = point;
                }
            }

            // Find the contact point on box2
            Vector3 contactPoint2 = Vector3.Zero;
            float minProjection2 = float.MaxValue;

            foreach(Vector3 point in box2.GetPoints())
            {
                float projection = Vector3.Dot(point, collisionNormal);
                if(projection < minProjection2)
                {
                    minProjection2 = projection;
                    contactPoint2 = point;
                }
            }

            // Return the midpoint of the two contact points
            return (contactPoint1 + contactPoint2) / 2;
        }

        private Vector3 CalculateNormal(BoxCollider box1, BoxCollider box2)
        {
            // Calculate the vector from box1 to box2
            Vector3 vector = box2.Body.Position - box1.Body.Position;

            vector.Normalize();

            return vector;
        }*/