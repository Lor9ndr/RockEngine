using RockEngine.Engine.EngineStates;

namespace RockEngine.Rendering.Layers
{
    public abstract class ALayer
    {
        public abstract int Order { get; }
        public abstract void OnRender();
    }
}
