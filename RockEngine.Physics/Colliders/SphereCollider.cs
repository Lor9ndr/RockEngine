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

       

        public override ref Vector3[] GetVertices()
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

        public override Vector3 GetLocalInertiaTensor(float mass)
        {
            // Assuming the object is a sphere with radius r
            float r = Radius;

            // Calculate the inertia tensor for a sphere
            float i = 2f / 5f * mass * r * r;

            // The inverse inertia tensor is the inverse of the inertia tensor
            return new Vector3(i);
        }
    }
}
