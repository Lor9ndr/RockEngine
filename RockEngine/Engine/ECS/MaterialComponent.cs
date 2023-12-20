using RockEngine.Assets;
using RockEngine.Editor;
using RockEngine.Engine.ECS.GameObjects;

using RockEngine.OpenGL;
using RockEngine.Rendering;

namespace RockEngine.Engine.ECS
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

        public void Render()
        {
            Material.SendData();
        }

        public void OnStart() { }

        public void OnUpdate()
        {
        }

        public void OnDestroy()
        {
        }

        public object GetState()
        {
            return new MaterialComponentState()
            {
                Material = Material,
            };
        }

        public void SetState(object state)
        {
            var ms = (MaterialComponentState)state;
            Material = ms.Material;
        }
    }
}
