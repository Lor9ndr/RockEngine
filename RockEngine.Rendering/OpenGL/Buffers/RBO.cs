using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.Common.Utils;
using RockEngine.Rendering.OpenGL.Settings;

namespace RockEngine.Rendering.OpenGL.Buffers
{
    public sealed class RBO : ASetuppableGLObject<RenderBufferSettings>
    {
        public override bool IsSetupped => Handle != IGLObject.EMPTY_HANDLE;

        public Vector2i Size { get; private set; }

        public RBO(RenderBufferSettings settings, Vector2i size)
            : base(settings)
        {
            Size = size;
        }

        public override RBO Bind(IRenderingContext context)
        {
            context.Bind(this);
            return this;
        }

        public override RBO Setup(IRenderingContext context)
        {
            context.CreateRenderBuffer(out int handle);
            Handle = handle;
            if(Settings.IsMultiSample)
            {
                context.NamedRenderbufferStorageMultisample(Handle, Settings.SampleCount, Settings.RenderbufferStorage, Size.X, Size.Y);
            }
            else
            {
                context.NamedRenderbufferStorage(Handle, Settings.RenderbufferStorage, Size.X, Size.Y);
            }
            return this;
        }

        public override RBO Unbind(IRenderingContext context)
        {
            context.BindRenderBuffer(Settings.RenderbufferTarget, IGLObject.EMPTY_HANDLE);
            return this;
        }

        public override RBO SetLabel(IRenderingContext context)
        {
            string label = $"RBO: ({Handle})";
            Logger.AddLog($"Setupped {label}");
            context.ObjectLabel(ObjectLabelIdentifier.Renderbuffer, Handle, label.Length, label);
            return this;
        }

        public void Resize(IRenderingContext context, Vector2i size)
        {
            Size = size;
            Dispose();
            IRenderingContext.Update(ctx=>Setup(ctx));
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

                GL.GetObjectLabel(ObjectLabelIdentifier.Renderbuffer, Handle, 128, out int _, out string name);
                if(name.Length == 0)
                {
                    name = $"RBO: ({Handle})";
                }
                Logger.AddLog($"Disposing {name}");
                context.DeleteRenderBuffer(_handle);
                _handle = IGLObject.EMPTY_HANDLE;
            });
            
        }

        public override bool IsBinded(IRenderingContext context)
        {
            context.GetInteger(GetPName.RenderbufferBinding, out int value);
            return value == Handle;
        }
    }
}
