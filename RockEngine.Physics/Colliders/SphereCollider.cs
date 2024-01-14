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

       

        public override ref Vector3[ ] GetVertices()
        {
            throw new NotImplementedException();
        }

        public override CollisionResult CheckCollision(BoxCollider otherCollider)
        {
            throw new NotImplementedException();
        }

        public override CollisionResult CheckCollision(SphereCollider otherCollider)
        {
            throw new NotImplementedException();
        }
    }
}
