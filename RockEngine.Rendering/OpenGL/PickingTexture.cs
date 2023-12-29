using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.Rendering.OpenGL.Buffers;
using RockEngine.Rendering.OpenGL.Settings;
using RockEngine.Rendering.OpenGL.Textures;
using RockEngine.Common.Utils;

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

            _fbo = new FBO(new FrameBufferSettings(FramebufferTarget.Framebuffer), Size, _pickingTexture, _depthTexture)
                .Setup()
                .SetLabel()
                .Bind();
            GL.ReadBuffer(ReadBufferMode.None);
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
            _fbo.CheckBuffer();
            _fbo.Unbind();
        }

        public void BeginWrite()
        {
            _fbo.BindAsDrawBuffer();
            GL.Viewport(0, 0, Size.X, Size.Y);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
            GL.ClearColor(0, 0, 0, 0);
        }

        public void EndWrite()
        {
            _fbo.UnbindAsDrawBuffer();
        }

        public void ReadPixel(int x, int y, ref PixelInfo info)
        {
            _fbo.ReadPixel(x, y, ref info);
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
