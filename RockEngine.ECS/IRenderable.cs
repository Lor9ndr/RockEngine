using RockEngine.Common;
using RockEngine.Rendering;

namespace RockEngine.ECS
{
    public interface IRenderable
    {
        void Render(IRenderingContext context);
    }
}
