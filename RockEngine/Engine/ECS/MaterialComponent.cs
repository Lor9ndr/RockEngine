using OpenTK.Mathematics;

using RockEngine.Assets;

using RockEngine.Engine.ECS.GameObjects;
using RockEngine.OpenGL.Buffers.UBOBuffers;
using RockEngine.OpenGL.Shaders;

using RockEngine.Editor;

namespace RockEngine.Engine.ECS
{
    public sealed class MaterialComponent : BaseAsset, IComponentRenderable<MaterialData>
    {
        public GameObject? Parent { get; set; }

        public int Order => 1;

        [UI(isColor: true)] public Vector3 AlbedoColor;
        [UI] public float Metallic;
        [UI] public float Roughness;
        [UI] public float Ao;
        public MaterialComponent( string path, string name, Guid id)
            : base(path, name, id, AssetType.Material)
        {
            AlbedoColor = new Vector3(0.5f, 1.0f, 1.0f);
            Metallic = 1;
            Roughness = 1;
            Ao = 0;
        }
        public MaterialComponent( string path, string name)
           : base(path, name, Guid.NewGuid(), AssetType.Material)
        {
            AlbedoColor = new Vector3(0.5f, 1.0f, 1.0f);
            Metallic = 1;
            Roughness = 1;
            Ao = 0;

        }
        public MaterialComponent()
            : base(PathInfo.PROJECT_PATH, "Material", Guid.NewGuid(), AssetType.Material)
        {
        }

        public void Render()
            => GetUBOData().SendData();

        public MaterialData GetUBOData()
            => new MaterialData()
            {
                albedo = AlbedoColor,
                metallic = Metallic,
                ao = Ao,
                roughness = Roughness,
            };

        public void RenderOnEditorLayer()
        {
            Render();
        }

        public void OnStart()
        {
        }

        public void OnUpdate()
        {
        }

        public void OnDestroy()
        {
        }
    }
}
