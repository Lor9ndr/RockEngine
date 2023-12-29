using OpenTK.Mathematics;

using RockEngine.Common.Editor;
using RockEngine.Rendering.OpenGL.Buffers.UBOBuffers;

namespace RockEngine.ECS.GameObjects
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
            LightColor = new Vector3(1);
            Intensity = 10.0f;
        }

        public DirectLight(Vector3 lightColor, float intensity)
        {
            LightColor = lightColor;
            Intensity = intensity;
        }

        public int Order => 0;

        public void OnDestroy()
        {
        }

        public void OnStart()
        {
            _lightData = new LightData();
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

        public dynamic GetState()
        {
            return new 
            {
                Intensity = Intensity,
                LightColor = LightColor,
                _lightData = _lightData,
            };
        }

        public void SetState(dynamic state)
        {
            Intensity = state.Intensity;
            LightColor = state.LightColor;
            _lightData = state._lightData;
        }
    }
}
