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

        public override FBORBO SetLabel(IRenderingContext context)
        {
            string label = $"FBO_RBO: {Settings.FramebufferTarget}({Handle})";
            Logger.AddLog($"Setupped {label}");
            context.ObjectLabel(ObjectLabelIdentifier.Framebuffer, Handle, label.Length, label);
            return this;
        }

        protected override FBORBO SetupInternal(IRenderingContext context)
        {
            foreach(var texture in _textures)
            {
                if(!texture.IsSetupped)
                {
                    texture
                        .Setup(context)
                        .SetLabel(context);
                }
                texture.Resize(context, Size);

                IRenderingContext.Update(context =>
                {
                    context.NamedFramebufferTexture(Handle, texture.Settings.FramebufferAttachment, texture.Handle, 0);

                });
            }
            IRenderingContext.Update(context =>
            {
                context.NamedFramebufferRenderbuffer(Handle, ((FrameBufferRenderBufferSettings)Settings).RenderBufferAttachment, _rbo.Settings.RenderbufferTarget, _rbo.Handle);
                CheckBuffer(context);

            });
            return this;
        }
        public override void Resize(IRenderingContext context, Vector2i size)
        {
            Size = size;
            _rbo.Resize(context, size);
            SetupInternal(context);
        }

        public override FBORBO Bind(IRenderingContext context)
        {
            base.Bind(context);

            return this;
        }

        public override FBORBO Unbind(IRenderingContext context)
        {
            base.Unbind(context);
            return this;
        }

        protected override void Dispose(bool disposing)
        {
            IRenderingContext.Update(context =>
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
                context.DeleteFrameBuffer(_handle);
                _handle = IGLObject.EMPTY_HANDLE;
            });
            // Вызываем отдельно Dispose для RBO так как нужно чтобы создалась отдельная команда для него
            _rbo.Dispose();

        }
    }
}
