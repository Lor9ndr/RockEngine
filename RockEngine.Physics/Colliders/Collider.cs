namespace RockEngine.Physics.Colliders
{
    public abstract class Collider
    {
        public RigidBody? Body { get; set; }
        public float Restitution = 0.5f;

        public Collider()
        {
        }

        public abstract bool CheckCollision(BoxCollider otherCollider, out CollisionResult result);
        public abstract bool CheckCollision(SphereCollider otherCollider, out CollisionResult result);

        internal bool CheckCollision(Collider collider, out CollisionResult result)
        {
            if(collider is BoxCollider bx)
            {
                return CheckCollision(bx, out result);
            }
            else if(collider is SphereCollider sc)
            {
                return CheckCollision(sc, out result);
            }
            result = new CollisionResult();
            return false;
        }
    }
}