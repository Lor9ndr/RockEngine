namespace RockEngine.ECS.Layers
{
    public abstract class ALayer
    {
        public abstract int Order { get; }
        public abstract void OnRender(Scene scene);
    }
}
