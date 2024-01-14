using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.Common.Utils;
using RockEngine.Rendering.OpenGL.Settings;
using RockEngine.Rendering.OpenGL.Textures;

namespace RockEngine.Rendering.OpenGL.Buffers
{
    public sealed class FBORBO : FBO
    {
        private readonly RBO _rbo;
        public FBORBO(FrameBufferRenderBufferSettings settings, Vector2i size, RBO rbo, params Texture[ ] textures)
            : base(settings, size, textures)
        {
            _rbo = rbo;
        }

        public override bool IsSetupped => Handle != IGLObject.EMPTY_HANDLE;

        public override FBORBO SetLabel()
        {
            string label = $"FBO_RBO: {Settings.FramebufferTarget}({Handle})";
            Logger.AddLog($"Setupped {label}");
            GL.ObjectLabel(ObjectLabelIdentifier.Framebuffer, Handle, label.Length, label);
            return this;
        }

        protected override FBORBO SetupInternal()
        {
            foreach(var texture in _textures)
            {
                if(!texture.IsSetupped)
                {
                    texture
                        .Setup()
                        .SetLabel();
                }
                texture.Resize(Size);

                GL.NamedFramebufferTexture(Handle, texture.Settings.FramebufferAttachment, texture.Handle, 0);
            }
            GL.NamedFramebufferRenderbuffer(Handle, ((FrameBufferRenderBufferSettings)Settings).RenderBufferAttachment, _rbo.Settings.RenderbufferTarget, _rbo.Handle);
            CheckBuffer();
            return this;
        }
        public override void Resize(Vector2i size)
        {
            Size = size;
            _rbo.Resize(size);
            SetupInternal();
        }

        public override FBORBO Bind()
        {
            base.Bind();

            return this;
        }

        public override FBORBO Unbind()
        {
            base.Unbind();
            return this;
        }

        protected override void Dispose(bool disposing)
        {
            if(_disposed)
            {
                return;
            }
            if(disposing)
            {
                // Освободите управляемые ресурсы здесь
            }

            if(!IsSetupped)
            {
                return;
            }
            GL.DeleteFramebuffers(1, ref _handle);
            _rbo.Dispose();
            _handle = IGLObject.EMPTY_HANDLE;
        }
    }
}
