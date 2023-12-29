using OpenTK.Mathematics;

namespace RockEngine.Physics
{
    public class SphereCollider : Collider
    {
        public float Radius { get; set; }

        public SphereCollider(float radius) : base()
        {
            Radius = radius;
        }

        public override bool CheckCollision(BoxCollider otherCollider, out Vector3 collisionPoint, out Vector3 normal)
        {
/*            Vector3 closestPoint = otherCollider.ClosestPoint(Body.Position);

            // Calculate the distance between the closest point and the sphere's center
            float distance = Vector3.Distance(closestPoint, Body.Position);

            // Check if the distance is less than or equal to the sphere's radius
            if(distance <= Radius)
            {
                // Collision has occurred
                collisionPoint = closestPoint;
                // Calculate the normal vector
                normal = (closestPoint - otherCollider.Body.Position).Normalized();
                return true;
            }*/

            // Calculate the normal vector
            normal = default;
            collisionPoint = default;
            return false;
        }

        public override bool CheckCollision(SphereCollider otherCollider, out Vector3 collisionPoint, out Vector3 normal)
        {
            // Calculate the distance between the centers of the two spheres
            Vector3 centerOffset = otherCollider.Body.Position - Body.Position;
            float distance = centerOffset.Length;

            // Check if the distance is less than or equal to the sum of the radii
            if(distance <= Radius + otherCollider.Radius)
            {
                // Collision has occurred
                // Calculate the collision point as the average of the two centers
                Vector3 closestPoint = otherCollider.ClosestPoint(Body.Position);
                normal = (closestPoint - otherCollider.Body.Position).Normalized();
                collisionPoint = (Body.Position + otherCollider.Body.Position) / 2f;
                return true;
            }

            collisionPoint = default;
            normal = default;
            return false;
        }

        public  Vector3 ClosestPoint(Vector3 position)
        {
            throw new NotImplementedException();
        }
    }
}
