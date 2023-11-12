using RockEngine.Engine;

namespace RockEngine.Rendering.Layers
{
    public abstract class ALayer
    {
        public abstract Layer Layer { get;}

        public abstract int Order { get; }
        public abstract void OnRender(Scene scene);
    }
}
