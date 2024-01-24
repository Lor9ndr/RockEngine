namespace RockEngine.Physics.Colliders
{
    public class ColliderMaterial
    {
        public float Restitution;

        public ColliderMaterial(float restitution)
        {
            Restitution = restitution;
        }

        public static ColliderMaterial Default => new ColliderMaterial(0.5f);
    }
}
