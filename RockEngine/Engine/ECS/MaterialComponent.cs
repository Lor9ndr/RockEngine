using OpenTK.Mathematics;

using RockEngine.Assets;

using RockEngine.Engine.ECS.GameObjects;
using RockEngine.OpenGL.Buffers.UBOBuffers;
using RockEngine.OpenGL.Shaders;

using RockEngine.Editor;

namespace RockEngine.Engine.ECS
{
    internal sealed class MaterialComponent : BaseAsset, IComponentRenderable<MaterialData>
    {
        public GameObject? Parent { get; set; }
        public AShaderProgram Shader { get; set; }

        public int Order => 1;

        [UI(isColor: true)] public Vector3 AmbientColor;
        [UI(isColor: true)] public Vector3 SpecularColor;
        [UI(isColor: true)] public Vector3 DiffuseColor;
        [UI] public float Shininess;
        public MaterialComponent(AShaderProgram shader, string path, string name, Guid id)
            : base(path, name, id, AssetType.Material)
        {
            Shader = shader;
            Shininess = 32.0f;
            AmbientColor = new Vector3(0.5f);
            DiffuseColor = new Vector3(0.2f);
            SpecularColor = new Vector3(1.0f);
        }
        public MaterialComponent(AShaderProgram shader, string path, string name)
           : base(path, name, Guid.NewGuid(), AssetType.Material)
        {
            Shader = shader;
            Shininess = 32.0f;
            AmbientColor = new Vector3(0.5f);
            DiffuseColor = new Vector3(0.2f);
            SpecularColor = new Vector3(1.0f);

        }
        public MaterialComponent()
            : base(PathInfo.PROJECT_PATH, "Material", Guid.NewGuid(), AssetType.Material)
        {
            Shader = AShaderProgram.AllShaders.First().Value;
        }

        public void Render()
            => GetUBOData().SendData();

        public MaterialData GetUBOData()
            => new MaterialData()
            {
                AmbientColor = AmbientColor,
                SpecularColor = SpecularColor,
                DiffuseColor = DiffuseColor,
                Shininess = Shininess,
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
