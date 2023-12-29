using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using RockEngine.Rendering.OpenGL.Shaders;
using RockEngine.Rendering.OpenGL.Buffers.UBOBuffers;
using RockEngine.Rendering.OpenGL;
using RockEngine.Common;
using RockEngine.ECS;
using RockEngine.Common.Utils;

namespace RockEngine.Renderers
{
    public sealed class PickingRenderer : IRenderer, IDisposable
    {
        private readonly PickingTexture PickingTexture;
        private readonly AShaderProgram PickingShader;

        public PickingRenderer(Vector2i size)
        {
            var basePicking = Path.Combine(PathConstants.RESOURCES, PathConstants.SHADERS, PathConstants.DEBUG, PathConstants.PICKING);
            PickingTexture = new PickingTexture(size);
            PickingShader = ShaderProgram.GetOrCreate("PickingShader",
               new VertexShader(Path.Combine(basePicking, "Picking.vert")),
             new FragmentShader(Path.Combine(basePicking, "Picking.frag")));
        }

        public void Begin()
        {
            PickingShader.BindIfNotBinded();
            GL.Disable(EnableCap.Blend);
            PickingTexture.BeginWrite();
        }

        public void Render(GameObject go)
        {
            PickingData pd = new PickingData()
            {
                gObjectIndex = go.GameObjectID,
                gDrawIndex = 0,
            };
            pd.SendData();
            go.Render();
        }

        public void End()
        {
            PickingTexture.EndWrite();
            PickingShader.Unbind();
        }

        public void Render(IComponent component)
        {
            throw new NotImplementedException("Unable to pick a component");
        }

        public void Dispose()
        {
        }

        public void ResizeTexture(Vector2i size)
        {
            PickingTexture.CheckSize(size);
        }

        public void ReadPixel(int x, int y, ref PixelInfo pixelInfo)
        {
            PickingTexture.ReadPixel(x, y, ref pixelInfo);
        }
    }
}
