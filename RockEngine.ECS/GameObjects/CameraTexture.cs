﻿using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.Common.Vertices;
using RockEngine.Rendering.OpenGL.Buffers;
using RockEngine.Rendering.OpenGL.Settings;
using RockEngine.Rendering.OpenGL.Shaders;
using RockEngine.Rendering.OpenGL.Textures;

namespace RockEngine.ECS.GameObjects
{
    public sealed class CameraTexture : IDisposable
    {
        public Texture ScreenTexture;
        private readonly FBORBO _screenFrameBuffer;
        private readonly Sprite _screenSprite;
        private readonly AShaderProgram _screenShader;
        private readonly RBO _screenRenderBuffer;

        private readonly Dictionary<TextureParameterName, int> _screenParameters =
           new Dictionary<TextureParameterName, int>()
           {
                     { TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Nearest},
                     { TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Nearest},
                     { TextureParameterName.TextureWrapS, (int) TextureWrapMode.ClampToEdge},
                     { TextureParameterName.TextureWrapT, (int) TextureWrapMode.ClampToEdge},
                     { TextureParameterName.TextureWrapR, (int) TextureWrapMode.ClampToEdge},
           };

        private static readonly Vertex2D[ ] _vertices = new Vertex2D[6]
           {
                    new Vertex2D(new Vector2(-1.0f,  1.0f), new Vector2(0.0f, 1.0f)),
                    new Vertex2D(new Vector2(-1.0f, -1.0f), new Vector2(0.0f, 0.0f)),
                    new Vertex2D(new Vector2( 1.0f, -1.0f), new Vector2(1.0f, 0.0f)),
                    new Vertex2D(new Vector2(-1.0f,  1.0f), new Vector2(0.0f, 1.0f)),
                    new Vertex2D(new Vector2( 1.0f, -1.0f), new Vector2(1.0f, 0.0f)),
                    new Vertex2D(new Vector2( 1.0f,  1.0f), new Vector2(1.0f, 1.0f))
           };
        public CameraTexture(Vector2i size)
        {
            ScreenTexture = new Texture(size, TextureSettings.DefaultSettings with
            {
                TextureParameters = _screenParameters,
                FramebufferAttachment = FramebufferAttachment.ColorAttachment0
            }).Setup();

            _screenShader = ShaderProgram.GetOrCreate("Lit2DShader",
                new VertexShader("Resources\\Shaders\\Screen\\Screen.vert"),
                new FragmentShader("Resources\\Shaders\\Screen\\Screen.frag"));

            _screenRenderBuffer = new RBO(new RenderBufferSettings(RenderbufferStorage.Depth32fStencil8, RenderbufferTarget.Renderbuffer), size).Setup();

            _screenFrameBuffer = ((FBORBO)new FBORBO(new FrameBufferRenderBufferSettings(FramebufferAttachment.DepthStencilAttachment, FramebufferTarget.Framebuffer),
                                            size,
                                            _screenRenderBuffer,
                                            ScreenTexture).Setup()).SetLabel();

            _screenSprite = new Sprite(ScreenTexture, in _vertices);
        }

        public void BeginRenderToScreen()
        {
            _screenFrameBuffer.Bind();
            GL.Enable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Viewport(0, 0, _screenFrameBuffer.Size.X, _screenFrameBuffer.Size.Y);
        }

        public void EndRenderToScreen()
        {
            _screenFrameBuffer.Unbind();
            GL.Disable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        }

        public void DisplayScreen()
        {
            _screenShader.Bind();
            _screenShader.SetShaderData("sampler", 1);
            _screenSprite.SpriteTexture.SetTextureUnit(1);
            _screenSprite.Render();
            _screenShader.Unbind();
        }

        public void Resize(Vector2i size)
        {
            if(_screenFrameBuffer.Size != size)
            {
                size = new Vector2i(Math.Clamp(size.X, 1, int.MaxValue), Math.Clamp(size.Y, 1, int.MaxValue));
                _screenFrameBuffer.Resize(size);
            }
        }

        public void Dispose()
        {
            ScreenTexture.Dispose();
            _screenShader.Dispose();
            _screenSprite.Dispose();
            _screenFrameBuffer.Dispose();
        }
    }
}