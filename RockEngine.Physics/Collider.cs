using OpenTK.Mathematics;

namespace RockEngine.Physics
{
    public abstract class Collider
    {
        public RigidBody? Body { get; set; }
        public float Restitution { get; set; } = 0.5f;

        public Collider()
        {
        }

        public abstract bool CheckCollision(BoxCollider otherCollider, out Vector3 collisionPoint, out Vector3 normal);
        public abstract bool CheckCollision(SphereCollider otherCollider, out Vector3 collisionPoint, out Vector3 normal);

        internal bool CheckCollision(Collider collider, out Vector3 collisionPoint, out Vector3 normal)
        {
            if(collider is BoxCollider bx)
            {
                return CheckCollision(bx, out collisionPoint, out normal);
            }
            else if(collider is SphereCollider sc)
            {
                return CheckCollision(sc, out collisionPoint, out normal);
            }
            collisionPoint = default;
            normal = default;
            return false;
        }
    }
}