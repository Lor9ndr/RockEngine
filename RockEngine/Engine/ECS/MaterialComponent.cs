using OpenTK.Mathematics;

using RockEngine.Assets;

using RockEngine.Engine.ECS.GameObjects;

using RockEngine.Editor;
using RockEngine.OpenGL;
using RockEngine.OpenGL.Buffers.UBOBuffers;

namespace RockEngine.Engine.ECS
{
    public sealed class MaterialComponent : IComponent, IRenderable
    {
        public Material Material;
        public MaterialData MaterialData;
        public GameObject? Parent { get; set; }

        public int Order => 1;

        [UI(isColor: true)] public Vector3 AlbedoColor;
        [UI] public float Metallic;
        [UI] public float Roughness;
        [UI] public float Ao;

        public MaterialComponent(Material material)
        {
            Material = material;
            AlbedoColor = new Vector3(0.7f);
            Metallic = 1.0f;
            Roughness = 1.0f;
            Ao = 1.0f;
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
            MaterialData.albedo = AlbedoColor;
            MaterialData.metallic = Metallic;
            MaterialData.roughness = Roughness;
            MaterialData.ao = Ao;
        }

        public void OnDestroy()
        {
        }

        public void OnUpdateDevelepmentState()
        {
            MaterialData.albedo = AlbedoColor;
            MaterialData.metallic = Metallic;
            MaterialData.roughness = Roughness;
            MaterialData.ao = Ao;
        }
    }
}
