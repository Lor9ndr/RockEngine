using OpenTK.Mathematics;

using RockEngine.OpenGL.Buffers.UBOBuffers;

using RockEngine.Editor;
using RockEngine.OpenGL;

namespace RockEngine.Engine.ECS.GameObjects
{
    public sealed class DirectLight : IComponent, IRenderable
    {
        private LightData _lightData;

        [UI(isColor: true)]
        public Vector3 LightColor;

        [UI] public float Intensity;
        public GameObject? Parent { get; set; }

        public DirectLight()
        {
            _lightData = new LightData();
            LightColor = new Vector3(1);
            Intensity = 10.0f;
        }

        public DirectLight(Vector3 lightColor, float intensity)
        {
            _lightData = new LightData();
            LightColor = lightColor;
            Intensity = intensity;
        }

        public int Order => 0;

        public void OnDestroy()
        {
        }

        public void OnStart()
        {
        }

        public void OnUpdate()
        {
            _lightData.lightColor = LightColor;
            _lightData.lightPosition = Parent!.Transform.Position;
            _lightData.lightDirection = Parent!.Transform.Rotation;
            _lightData.intensity = Intensity;
        }

        public void Render()
        {
            _lightData.SendData();
        }

        public void RenderOnEditorLayer()
        {
            Render();
        }

        public void OnUpdateDevelepmentState()
        {
            OnUpdate();
        }
    }
}
