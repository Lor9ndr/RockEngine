using OpenTK.Mathematics;

using RockEngine.OpenGL.Buffers.UBOBuffers;

using RockEngine.Editor;

namespace RockEngine.Engine.ECS.GameObjects
{
    internal sealed class DirectLight : IComponentRenderable<LightData>
    {
        [UI(isColor: true)]
        public Vector3 LightColor;
        [UI]
        public float Intensity;

        public GameObject Parent { get; set; }

        public int Order => 0;

        public LightData GetUBOData()
        {
            return new LightData()
            {
                LightColor = LightColor,
                LightPosition = Parent.Transform.Position,
                LightDirection = Parent.Transform.Rotation,
                Intensity = Intensity
            };
        }

        public void OnDestroy()
        {
        }

        public void OnStart()
        {
        }

        public void OnUpdate()
        {
        }

        public void Render()
        {
            GetUBOData().SendData();
        }

        public void RenderOnEditorLayer()
        {
        }
    }
}
