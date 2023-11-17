using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.OpenGL.Buffers;
using RockEngine.OpenGL.Settings;
using RockEngine.OpenGL.Textures;

namespace RockEngine.OpenGL
{
    public sealed class PickingTexture : IDisposable
    {
        public Vector2i Size { get; private set;}
        private FBO _fbo;
        private Texture _pickingTexture;
        private Texture _depthTexture;

        public struct PixelInfo
        {
            public float ObjectID;
            public float DrawID;
            public float PrimID;

            public PixelInfo()
            {
                ObjectID = 0.0f;
                DrawID = 0.0f;
                PrimID = 0.0f;
            }
        };

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
                .Setup().SetLabel().Bind();
            GL.ReadBuffer(ReadBufferMode.None);
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
            _fbo.CheckBuffer();
            _fbo.Unbind();

        }

        public void BeginWrite()
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _fbo.Handle);
        }

        public void EndWrite()
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, IGLObject.EMPTY_HANDLE);
        }

        public PixelInfo ReadPixel(int x, int y)
        {
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, _fbo.Handle);
            GL.ReadBuffer(ReadBufferMode.ColorAttachment0);

            PixelInfo pixel = new PixelInfo();
            GL.ReadPixels(x, y, 1, 1, PixelFormat.Rgb, PixelType.Float, ref pixel);

            GL.ReadBuffer(ReadBufferMode.None);
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, IGLObject.EMPTY_HANDLE);
            return pixel;
        }

        public void CheckSize(Vector2i size)
        {
            if (size != Size)
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
