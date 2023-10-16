using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.OpenGL.Buffers;
using RockEngine.OpenGL.Settings;
using RockEngine.OpenGL.Textures;

namespace RockEngine.OpenGL
{
    internal sealed class PickingTexture : IDisposable
    {
        private Vector2i _size;
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
                ObjectID = 0;
                DrawID = 0;
                PrimID = 0;
            }
        };

        public PickingTexture(Vector2i size)
        {
            _size = size;
            Setup();
        }

        private void Setup()
        {
            _pickingTexture = new Texture(_size, new TextureSettings()
            {
                TextureTarget = TextureTarget.Texture2D,
                SizedInternalFormat = SizedInternalFormat.Rgb32f,
                FramebufferAttachment = FramebufferAttachment.ColorAttachment0,
                PixelType = PixelType.Float
            });

            _depthTexture = new Texture(_size, new TextureSettings()
            {
                TextureTarget = TextureTarget.Texture2D,
                SizedInternalFormat = SizedInternalFormat.DepthComponent32,
                PixelType = PixelType.Float,
                FramebufferAttachment = FramebufferAttachment.DepthAttachment
            });

            _fbo = new FBO(new FrameBufferSettings(FramebufferTarget.Framebuffer), _size, _pickingTexture, _depthTexture)
                .Setup().SetLabel().Bind();
            GL.ReadBuffer(ReadBufferMode.None);
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
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
            if (size != _size)
            {
                _size = size;
                _fbo.Resize(size);

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
