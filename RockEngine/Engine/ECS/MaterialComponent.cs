using RockEngine.Assets;

using RockEngine.Engine.ECS.GameObjects;

using RockEngine.OpenGL;
using RockEngine.OpenGL.Buffers.UBOBuffers;
using RockEngine.Utils;

namespace RockEngine.Engine.ECS
{
    public sealed class MaterialComponent : IComponent, IRenderable
    {
        public Material Material;
        public MaterialData MaterialData;
        public GameObject? Parent { get; set; }

        public int Order => 1;
       

        public MaterialComponent(Material material)
        {
            Validator.ThrowIfNull(material);
            Material = material;
           
            MaterialData = new MaterialData();
        }

        public void Render() => MaterialData.SendData();

        public void RenderOnEditorLayer()
        {
            Render();
        }

        public void OnStart()
        {
        }

        public void OnUpdate()
        {
            MaterialData.albedo = Material.AlbedoColor;
            MaterialData.metallic = Material.Metallic;
            MaterialData.roughness = Material.Roughness;
            MaterialData.ao = Material.Ao;
        }

        public void OnDestroy()
        {
        }

        public void OnUpdateDevelepmentState()
        {
            MaterialData.albedo = Material.AlbedoColor;
            MaterialData.metallic = Material.Metallic;
            MaterialData.roughness = Material.Roughness;
            MaterialData.ao = Material.Ao;
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
