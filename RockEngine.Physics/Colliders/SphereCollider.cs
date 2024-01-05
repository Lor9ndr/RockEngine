using OpenTK.Mathematics;

namespace RockEngine.Physics.Colliders
{
    public class SphereCollider : Collider
    {
        public float Radius { get; set; }

        public SphereCollider(float radius) : base()
        {
            Radius = radius;
        }

        public override bool CheckCollision(BoxCollider otherCollider, out CollisionResult result)
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
            result = new CollisionResult();
            return false;
        }

        public override bool CheckCollision(SphereCollider otherCollider, out CollisionResult result)
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
                result = new CollisionResult();
                result.Normal = (closestPoint - otherCollider.Body.Position).Normalized();
                result.ContactPoint = (Body.Position + otherCollider.Body.Position) / 2f;
                result.PenetrationDepth = 0;
                return true;
            }

            result = new CollisionResult();
            return false;
        }

        public Vector3 ClosestPoint(Vector3 position)
        {
            throw new NotImplementedException();
        }
    }
}
