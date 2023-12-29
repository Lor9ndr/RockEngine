using OpenTK.Mathematics;

namespace RockEngine.Physics
{
    public class BoxCollider:Collider
    {
        public Vector3 Position;
        public Vector3 Extents;

        public Vector3 HalfExtents => Extents  / 2f;
        public BoxCollider(Vector3 position, Vector3 extents)
        {
            Position = position;
            Extents = extents;
        }

        public override bool CheckCollision(BoxCollider otherCollider, out Vector3 collisionPoint, out Vector3 normal)
        {
            Position = Body.Position;
            otherCollider.Position = otherCollider.Body.Position;

            if(Position.X < otherCollider.Position.X + otherCollider.HalfExtents.X &&
                Position.X > otherCollider.Position.X - otherCollider.HalfExtents.X &&
                Position.Y < otherCollider.Position.Y + otherCollider.HalfExtents.Y &&
                Position.Y > otherCollider.Position.Y - otherCollider.HalfExtents.Y &&
                Position.Z < otherCollider.Position.Z + otherCollider.HalfExtents.Z &&
                Position.Z > otherCollider.Position.Z - otherCollider.HalfExtents.Z)
            {
                collisionPoint = (Position - otherCollider.Position).Normalized()  / HalfExtents;
                normal = (collisionPoint - otherCollider.Position).Normalized();
                return true;
            }
            else
            {
                collisionPoint = Vector3.Zero;
                normal = Vector3.Zero;
                return false;
            }
        }

        public override bool CheckCollision(SphereCollider otherCollider, out Vector3 collisionPoint, out Vector3 normal)
            => otherCollider.CheckCollision(this, out collisionPoint, out normal);
    }
}
