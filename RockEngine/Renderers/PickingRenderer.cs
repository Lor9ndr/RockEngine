using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.Common;
using RockEngine.Common.Utils;
using RockEngine.ECS;
using RockEngine.Rendering;
using RockEngine.Rendering.OpenGL;
using RockEngine.Rendering.OpenGL.Buffers.UBOBuffers;
using RockEngine.Rendering.OpenGL.Shaders;

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
            IRenderingContext.Update((context) => PickingShader.Setup(context));
        }

        public void Begin(IRenderingContext context)
        {
            PickingShader.BindIfNotBinded(context);
            context.Disable(EnableCap.Blend);
            PickingTexture.BeginWrite(context);
        }

        public void Render(IRenderingContext context, GameObject go)
        {
            PickingData pd = new PickingData()
            {
                gObjectIndex = go.GameObjectID,
                gDrawIndex = 0,
            };
            pd.SendData(context);
            go.Render(context);
        }

        public void End(IRenderingContext context)
        {
            PickingTexture.EndWrite(context);
            PickingShader.Unbind(context);
        }

        public void Render(IRenderingContext context, IComponent component)
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

        public void ReadPixel(IRenderingContext context, int x, int y, ref PixelInfo pixelInfo)
        {
            PickingTexture.ReadPixel(context, x, y, ref pixelInfo);
        }
    }
}
