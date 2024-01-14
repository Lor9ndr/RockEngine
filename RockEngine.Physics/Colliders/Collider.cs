using OpenTK.Mathematics;

namespace RockEngine.Physics.Colliders
{
    public abstract class Collider
    {
        public RigidBody Body { get; set; }
        public float Restitution = 0.5f;
        public Vector3 Center => Body.Position;

        public Collider()
        {
        }

        public abstract ref Vector3[ ] GetVertices();

        public abstract CollisionResult CheckCollision(BoxCollider otherCollider);
        public abstract CollisionResult CheckCollision(SphereCollider otherCollider);

        internal CollisionResult CheckCollision(Collider collider)
        {
            if(collider is BoxCollider bx)
            {
                return CheckCollision(bx);
            }
            else if(collider is SphereCollider sc)
            {
                return CheckCollision(sc);
            }
            return new CollisionResult();
        }
    }
}