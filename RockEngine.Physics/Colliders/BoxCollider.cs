using OpenTK.Mathematics;

namespace RockEngine.Physics.Colliders
{
    public class BoxCollider : Collider
    {
        public Vector3 Min;
        public Vector3 Max;
        public Vector3 Extents => (Max-Min)/ 2.0f;

        public BoxCollider(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }

        public override bool CheckCollision(BoxCollider otherCollider, out CollisionResult result)
        {
            result = new CollisionResult();

            // Transform the local coordinates to world coordinates
            Vector3 thisMin = Vector3.Transform(Min,Body.Rotation) + Body.Position;
            Vector3 thisMax = Vector3.Transform(Max,Body.Rotation) + Body.Position;
            Vector3 otherMin = Vector3.Transform(otherCollider.Min, otherCollider.Body.Rotation) + otherCollider.Body.Position;
            Vector3 otherMax = Vector3.Transform(otherCollider.Max, otherCollider.Body.Rotation) + otherCollider.Body.Position;

            // Check for overlap on each axis
            float overlapX = Math.Min(thisMax.X, otherMax.X) - Math.Max(thisMin.X, otherMin.X);
            float overlapY = Math.Min(thisMax.Y, otherMax.Y) - Math.Max(thisMin.Y, otherMin.Y);
            float overlapZ = Math.Min(thisMax.Z, otherMax.Z) - Math.Max(thisMin.Z, otherMin.Z);

            // If there is no overlap on any axis, then there is no collision
            if(overlapX <= 0 || overlapY <= 0 || overlapZ <= 0)
            {
                return false;
            }

            // Find the axis of least penetration
            float minOverlap = Math.Min(overlapX, Math.Min(overlapY, overlapZ));

            // Set the collision normal based on the axis of least penetration
            Vector3 collisionNormal;
            if(minOverlap == overlapX)
            {
                collisionNormal = new Vector3(thisMax.X < otherMax.X ? -1 : 1, 0, 0);
            }
            else if(minOverlap == overlapY)
            {
                collisionNormal = new Vector3(0, thisMax.Y < otherMax.Y ? -1 : 1, 0);
            }
            else // minOverlap == overlapZ
            {
                collisionNormal = new Vector3(0, 0, thisMax.Z < otherMax.Z ? -1 : 1);
            }

            // Calculate the contact point as the midpoint of the box edges in the direction of the collision normal
            Vector3 contactPoint = Vector3.Zero;
            if(collisionNormal.X != 0)
            {
                contactPoint.Y = (thisMin.Y + thisMax.Y) / 2;
                contactPoint.Z = (thisMin.Z + thisMax.Z) / 2;
                contactPoint.X = collisionNormal.X == 1 ? thisMin.X : thisMax.X;
            }
            else if(collisionNormal.Y != 0)
            {
                contactPoint.X = (thisMin.X + thisMax.X) / 2;
                contactPoint.Z = (thisMin.Z + thisMax.Z) / 2;
                contactPoint.Y = collisionNormal.Y == 1 ? thisMin.Y : thisMax.Y;
            }
            else // collisionNormal.Z != 0
            {
                contactPoint.X = (thisMin.X + thisMax.X) / 2;
                contactPoint.Y = (thisMin.Y + thisMax.Y) / 2;
                contactPoint.Z = collisionNormal.Z == 1 ? thisMin.Z : thisMax.Z;
            }

            // Write the results to the collision result
            result.Normal = collisionNormal;
            result.PenetrationDepth = minOverlap;
            result.ContactPoint = contactPoint + Body.Position;

            return true;
        }

        public override bool CheckCollision(SphereCollider otherCollider, out CollisionResult result)
            => otherCollider.CheckCollision(this, out result);
    }
}
