using OpenTK.Mathematics;

namespace RockEngine.Physics.Colliders
{
    public abstract class Collider
    {
        public RigidBody Body { get; set; }
        public ColliderMaterial Material;
        public Collider()
        {
            Material = ColliderMaterial.Default;
        }
        public Collider(ColliderMaterial material)
        {
            Material = material;
        }

        public abstract ref Vector3[] GetVertices();
        public abstract CollisionResult CheckCollision(BoxCollider otherCollider);
        public abstract CollisionResult CheckCollision(SphereCollider otherCollider);

        public abstract Vector3 GetLocalInertiaTensor(float mass);

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