using RockEngine.ECS;

namespace RockEngine.Renderers
{
    public interface IRenderer
    {
        public void Render(GameObject go);
        public void Render(IComponent component);
    }
}
