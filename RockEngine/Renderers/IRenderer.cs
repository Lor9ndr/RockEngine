using RockEngine.Common;
using RockEngine.ECS;
using RockEngine.Rendering;

namespace RockEngine.Renderers
{
    public interface IRenderer
    {
        public void Render(IRenderingContext context, GameObject go);
        public void Render(IRenderingContext context, IComponent component);
    }
}
