using OpenTK.Mathematics;
using SimdLinq;
using RockEngine.Physics.CollisionResolution;
using RockEngine.Common.Utils;

namespace RockEngine.Physics.Colliders
{
    public class OBB : ICollider
    {
        public bool WasCollided { get; set; }
        public float Restitution { get; set; } = 0.5f;
        public Vector3 Center { get; private set; }
        public Vector3 Extents { get; private set; }
        public Quaternion Rotation { get; private set; }

        public OBB(Vector3 center, Vector3 extents)
        {
            Center = center;
            Extents = extents;
            Rotation = Quaternion.Identity;
        }

        public bool Accept(ICollider visitor, out CollisionManifold manifold)
        {
            return visitor.Visit(this, out manifold);
        }

        public Matrix3 CalculateInertiaTensor(float mass)
        {
            // Calculate the squared dimensions of the box
            float widthSquared = Extents.X * Extents.X;
            float heightSquared = Extents.Y * Extents.Y;
            float depthSquared = Extents.Z * Extents.Z;

            // Calculate the diagonal elements of the inertia tensor for an AABB
            float ix = (1.0f / 12.0f) * mass * (heightSquared + depthSquared);
            float iy = (1.0f / 12.0f) * mass * (widthSquared + depthSquared);
            float iz = (1.0f / 12.0f) * mass * (widthSquared + heightSquared);

            // Create and return the inertia tensor matrix
            Matrix3 inertiaTensor = new Matrix3(
                new Vector3(ix, 0, 0),
                new Vector3(0, iy, 0),
                new Vector3(0, 0, iz)
            );

            return inertiaTensor;
        }

        public AABB GetAABB() => new AABB(Center, Extents);

        public void GetUpdatesFromBody(RigidBody body)
        {
            Center = body.Position;
            Rotation = body.Rotation;
        }

        public bool Visit(AABB other, out CollisionManifold manifold)
        {
            throw new NotImplementedException();
        }

        public bool Visit(OBB other, out CollisionManifold manifold)
        {
            manifold = new CollisionManifold
            {
                Colliding = false,
                Normal = Vector3.Zero,
                Depth = float.MaxValue,
                ContactPoints = new List<Vector3>() // Placeholder for contact points
            };

            // Convert quaternions to rotation matrices for both OBBs
            Matrix3 rotationThis = Matrix3.CreateFromQuaternion(Rotation);
            Matrix3 rotationOther = Matrix3.CreateFromQuaternion(other.Rotation);

            // Compute the rotation matrix expressing 'other' in 'this' coordinate frame
            Matrix3 R = new Matrix3();
            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    R[i, j] = Vector3.Dot(rotationThis.GetRow(i), rotationOther.GetRow(j));
                }
            }

            // Compute translation vector t
            Vector3 t = other.Center - Center;
            // Bring translation into 'this' coordinate frame
            t = new Vector3(Vector3.Dot(t, rotationThis.Row0), Vector3.Dot(t, rotationThis.Row1), Vector3.Dot(t, rotationThis.Row2));

            // Compute common subexpressions. Add in an epsilon term to counteract arithmetic errors when two edges are parallel and their cross product is (near) null
            Matrix3 AbsR = new Matrix3();
            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    AbsR[i, j] = Math.Abs(R[i, j]) + float.Epsilon;
                }
            }

            float minOverlap = float.MaxValue;
            Vector3 minAxis = Vector3.Zero;

            float ra, rb;
            Matrix3 extentsThis = new Matrix3(Extents.X, 0, 0, 0, Extents.Y, 0, 0, 0, Extents.Z);
            Matrix3 extentsOther = new Matrix3(other.Extents.X, 0, 0, 0, other.Extents.Y, 0, 0, 0, other.Extents.Z);

            // Test axes L = A0, L = A1, L = A2
            for(int i = 0; i < 3; i++)
            {
                Vector3 axis = rotationThis.GetRow(i);
                var column = AbsR.GetColumn(i);
                ra = extentsThis[i, i];
                rb = Vector3.Dot(extentsOther.Row0, column) + Vector3.Dot(extentsOther.Row1, column) + Vector3.Dot(extentsOther.Row2, column);
                float overlap = Math.Abs(Vector3.Dot(t, axis)) - (ra + rb);
                if(overlap > 0)
                {
                    return false; // No overlap found, OBBs do not intersect
                }

                if(Math.Abs(overlap) < minOverlap)
                {
                    minOverlap = Math.Abs(overlap);
                    minAxis = axis;
                }
            }

            // Test axes L = B0, L = B1, L = B2
            for(int i = 0; i < 3; i++)
            {
                Vector3 axis = rotationOther.GetRow(i);
                var column = AbsR.GetColumn(i);
                ra = Vector3.Dot(extentsThis.Row0, column) + Vector3.Dot(extentsThis.Row1, column) + Vector3.Dot(extentsThis.Row2, column);
                rb = extentsOther[i, i];
                float overlap = Math.Abs(Vector3.Dot(t, R.GetColumn(i))) - (ra + rb);
                if(overlap > 0)
                {
                    return false; // No overlap found, OBBs do not intersect
                }

                if(Math.Abs(overlap) < minOverlap)
                {
                    minOverlap = Math.Abs(overlap);
                    minAxis = R.GetColumn(i);
                }
            }

            // Test axis L = A0 x B0, L = A0 x B1, L = A0 x B2, L = A1 x B0, ...
            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    Vector3 axis = Vector3.Cross(rotationThis.GetRow(i), rotationOther.GetRow(j));
                    if(axis.LengthSquared < float.Epsilon)
                    {
                        continue; // Skip near-parallel axes
                    }
                    axis = axis.Normalized(); // Ensure the axis is normalized before using it

                    float projectionA = ProjectOBB(this, axis, rotationThis);
                    float projectionB = ProjectOBB(other, axis, rotationOther);
                    float overlap = Math.Abs(Vector3.Dot(t, axis)) - (projectionA + projectionB);
                    if(overlap > 0)
                    {
                        return false; // No overlap found, OBBs do not intersect
                    }
                    if(Math.Abs(overlap) < minOverlap)
                    {
                        minOverlap = Math.Abs(overlap);
                        minAxis = axis;
                    }
                }
            }

            var normal = minAxis;
            var depth = minOverlap;

            // Calculate the world space points of both OBBs
            var pointsA = CalculateOBBPoints(Center, Extents, Rotation);
            var pointsB = CalculateOBBPoints(other.Center, other.Extents, other.Rotation);

            // Project all points of both OBBs onto the collision normal
            var projectionsA = pointsA.Select(point => Vector3.Dot(point, normal));
            var projectionsB = pointsB.Select(point => Vector3.Dot(point, normal));

            // Find the overlap region on the collision normal
            float minA = projectionsA.Min();
            float maxA = projectionsA.Max();
            float minB = projectionsB.Min();
            float maxB = projectionsB.Max();

            float overlapStart = Math.Max(minA, minB);
            float overlapEnd = Math.Min(maxA, maxB);

            // Find the points that are within the overlap region on the collision normal
            var contactPointsA = pointsA.Where(point => {
                float projection = Vector3.Dot(point, normal);
                return projection >= overlapStart && projection <= overlapEnd;
            });

            var contactPointsB = pointsB.Where(point => {
                float projection = Vector3.Dot(point, normal);
                return projection >= overlapStart && projection <= overlapEnd;
            });

            // Combine the contact points from both OBBs
            manifold.ContactPoints.AddRange(contactPointsA);
            manifold.ContactPoints.AddRange(contactPointsB);
            manifold.ContactPoints = manifold.ContactPoints.Distinct().ToList();
            manifold.Normal = normal;
            manifold.Depth = depth;
            manifold.Colliding = true;

            return true;
        }

        private float ProjectOBB(OBB obb, Vector3 axis, Matrix3 rotationMatrix)
        {
            // Project an OBB onto an axis and return the projection's half-length
            return obb.Extents.X * Math.Abs(Vector3.Dot(axis, rotationMatrix.Row0)) +
                   obb.Extents.Y * Math.Abs(Vector3.Dot(axis, rotationMatrix.Row1)) +
                   obb.Extents.Z * Math.Abs(Vector3.Dot(axis, rotationMatrix.Row2));
        }
        private List<Vector3> CalculateOBBPoints(Vector3 center, Vector3 extents, Quaternion rotation)
        {
            List<Vector3> points = new List<Vector3>(8);

            // Define the 8 corners of the OBB
            Span<Vector3> localPoints = stackalloc Vector3[]
                    {
                new Vector3(-extents.X, -extents.Y, -extents.Z),
                new Vector3(extents.X, -extents.Y, -extents.Z),
                new Vector3(extents.X, extents.Y, -extents.Z),
                new Vector3(-extents.X, extents.Y, -extents.Z),
                new Vector3(-extents.X, -extents.Y, extents.Z),
                new Vector3(extents.X, -extents.Y, extents.Z),
                new Vector3(extents.X, extents.Y, extents.Z),
                new Vector3(-extents.X, extents.Y, extents.Z)
            };

            // Transform the local points to world space
            foreach(var localPoint in localPoints)
            {
                Vector3 worldPoint = center + Vector3.Transform(localPoint, rotation);
                points.Add(worldPoint);
            }

            return points;
        }
    }
}