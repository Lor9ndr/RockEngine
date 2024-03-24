using RockEngine.Common;
using RockEngine.Common.Editor;
using RockEngine.ECS.Assets;
using RockEngine.Rendering;

namespace RockEngine.ECS
{
    public sealed class MaterialComponent : IComponent, IRenderable
    {
        [UI]
        public Material Material;
        public GameObject? Parent { get; set; }

        public int Order => 1;

        public MaterialComponent(Material material)
        {
            Material = material;
        }

        public void Render(IRenderingContext context)
        {
            Material.SendData(context);
        }

        public void OnStart() { }

        public void OnUpdate()
        {
        }

        public void OnDestroy()
        {
        }

        public dynamic GetState()
        {
            return new
            {
                Material = Material,
            };
        }

        public void SetState(dynamic state)
        {
            Material = state.Material;
        }
    }
}
