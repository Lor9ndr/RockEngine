using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.Common.Utils;
using RockEngine.Rendering.OpenGL.Buffers;
using RockEngine.Rendering.OpenGL.Settings;
using RockEngine.Rendering.OpenGL.Textures;

namespace RockEngine.Rendering.OpenGL
{
    public sealed class PickingTexture : IDisposable
    {
        public Vector2i Size { get; private set; }
        private FBO _fbo;
        private Texture _pickingTexture;
        private Texture _depthTexture;

        public PickingTexture(Vector2i size)
        {
            Size = size;
            Setup();
        }

        private void Setup()
        {
            _pickingTexture = new Texture(Size, new TextureSettings()
            {
                TextureTarget = TextureTarget.Texture2D,
                SizedInternalFormat = SizedInternalFormat.Rgb32f,
                FramebufferAttachment = FramebufferAttachment.ColorAttachment0,
                PixelType = PixelType.Float,
                TextureParameters = new Dictionary<TextureParameterName, int>()
                 {
                     { TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat},
                     { TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat},
                     { TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest},
                     { TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest},
                 }
            });

            _depthTexture = new Texture(Size, new TextureSettings()
            {
                TextureTarget = TextureTarget.Texture2D,
                SizedInternalFormat = SizedInternalFormat.DepthComponent16,
                PixelType = PixelType.Float,
                FramebufferAttachment = FramebufferAttachment.DepthAttachment
            });

            IRenderingContext.Update(context =>
            {
                _fbo = new FBO(new FrameBufferSettings(FramebufferTarget.Framebuffer), Size, _pickingTexture, _depthTexture)
                .Setup(context)
                .SetLabel(context)
                .Bind(context);

                context.ReadBuffer(ReadBufferMode.None)
                    .DrawBuffer(DrawBufferMode.ColorAttachment0);

                _fbo.CheckBuffer(context);
                _fbo.Unbind(context);

            });
           
        }

        public void BeginWrite(IRenderingContext context)
        {
            _fbo.BindAsDrawBuffer(context);
            context.Viewport(0, 0, Size.X, Size.Y)
                .Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit)
                .ClearColor(0, 0, 0, 0);
        }

        public void EndWrite(IRenderingContext context)
        {
            _fbo.UnbindAsDrawBuffer(context);
        }

        public void ReadPixel(IRenderingContext context, int x, int y, ref PixelInfo info)
        {
            _fbo.ReadPixel(context, x, y, ref info);
        }

        public void ReadPixel(IRenderingContext context, int x, int y, ref PixelInfo[ ] info)
        {
            _fbo.ReadPixel(context,x, y, ref info);
        }

        public void CheckSize(Vector2i size)
        {
            if(size != Size)
            {
                Size = size;
                Dispose();
                Setup();
            }
        }

        public void Dispose()
        {
            _fbo.Dispose();
            _pickingTexture.Dispose();
            _depthTexture.Dispose();
        }
    }
}
