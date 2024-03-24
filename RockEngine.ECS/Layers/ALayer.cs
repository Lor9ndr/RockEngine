using RockEngine.Rendering;

namespace RockEngine.ECS.Layers
{
    public abstract class ALayer
    {
        public abstract int Order { get; }
        public abstract Task OnRender(Scene scene);
    }
}
