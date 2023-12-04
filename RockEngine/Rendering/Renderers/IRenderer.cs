using RockEngine.Engine.ECS;
using RockEngine.Engine.ECS.GameObjects;

namespace RockEngine.Rendering.Renderers
{
    public interface IRenderer
    {
        public void Render(GameObject go);
        public void Render(IComponent component);
    }
}
