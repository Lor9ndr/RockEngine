using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.Common.Utils;
using RockEngine.Rendering.OpenGL.Settings;
using RockEngine.Rendering.OpenGL.Textures;

namespace RockEngine.Rendering.OpenGL.Buffers
{
    public class FBO : ASetuppableGLObject<FrameBufferSettings>
    {
        public Vector2i Size { get; protected set; }
        protected List<Texture> _textures;
        private static int _prevFbo;
        private static int _prevDrawFbo;
        private static int _prevReadFbo;

        public FBO(FrameBufferSettings settings, Vector2i size, params Texture[ ] textures)
            : base(settings)
        {
            Size = size;
            _textures = new List<Texture>();
            _textures.AddRange(textures);
        }

        public override bool IsSetupped => Handle != IGLObject.EMPTY_HANDLE;

        public override void Dispose(bool disposing, IRenderingContext context = null)
        {
            if(context is null)
            {
                IRenderingContext.Update(context =>
                {
                    InternalDispose(disposing, context);
                });
            }
            else
            {
                InternalDispose(disposing, context);
            }
        }

        private void InternalDispose(bool disposing, IRenderingContext context)
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

            context.GetObjectLabel(ObjectLabelIdentifier.Framebuffer, Handle, 64, out int length, out string name);
            if(name.Length == 0)
            {
                name = $"FBO: ({Handle})";
            }
            Logger.AddLog($"Disposing {name}");
            context.DeleteFrameBuffer(_handle);
            // now Handle is 0 
            _handle = IGLObject.EMPTY_HANDLE;
        }

        public override FBO SetLabel(IRenderingContext context)
        {
            string label = $"FBO: ({Handle})";
            Logger.AddLog($"Setupped {label}");
            context.ObjectLabel(ObjectLabelIdentifier.Framebuffer, Handle, label.Length, label);
            return this;
        }

        public override FBO Setup(IRenderingContext context)
        {
            context.CreateFrameBuffer(out int handle);
            Handle = handle;

            SetupInternal(context);
            return this;
        }

        protected virtual FBO SetupInternal(IRenderingContext context)
        {
            foreach(var texture in _textures)
            {
                texture.Resize(context, Size);
                if(!texture.IsSetupped)
                {
                    texture
                        .Setup(context)
                        .SetLabel(context);
                }
                context.NamedFramebufferTexture(Handle, texture.Settings.FramebufferAttachment, texture.Handle, 0);
            }

            CheckBuffer(context);
            return this;
        }

        public virtual void Resize(IRenderingContext context, Vector2i size)
        {
            Size = size;
            SetupInternal(context);
        }
        public void CheckBuffer(IRenderingContext context)
        {
            context.CheckNamedFramebufferStatus(Handle, Settings.FramebufferTarget, out var status);
            if(status != FramebufferStatus.FramebufferComplete)
            {
                Logger.AddError($"Can't create a framebuffer. Status : {status}");
            }
        }

        public override FBO Bind(IRenderingContext context)
        {
            context.GetInteger(GetPName.FramebufferBinding, out _prevFbo)
                .Bind(this);
            return this;
        }

        public override FBO Unbind(IRenderingContext context)
        {
            context.BindFBOAs(FramebufferTarget.Framebuffer, _prevFbo);
            return this;
        }

        public virtual FBO BindAsDrawBuffer(IRenderingContext context)
        {
            context.GetInteger(GetPName.DrawFramebufferBinding, out _prevDrawFbo)
                .BindFBOAs(FramebufferTarget.DrawFramebuffer, Handle);
            return this;
        }

        public virtual FBO UnbindAsDrawBuffer(IRenderingContext context)
        {
            context.BindFBOAs(FramebufferTarget.DrawFramebuffer, _prevDrawFbo);
            return this;
        }
        public virtual FBO BindAsReadBuffer(IRenderingContext context)
        {
            context.GetInteger(GetPName.ReadFramebufferBinding, out _prevReadFbo)
                .BindFBOAs(FramebufferTarget.ReadFramebuffer, Handle);
            return this;
        }

        public virtual FBO UnbindAsReadBuffer(IRenderingContext context)
        {
            context.BindFBOAs(FramebufferTarget.ReadFramebuffer, _prevReadFbo);
            return this;
        }

        public void ReadPixel(IRenderingContext context,int x, int y, ref PixelInfo info)
        {
            BindAsReadBuffer(context);

            context
                .ReadBuffer(ReadBufferMode.ColorAttachment0)
                .ReadPixel(x, y, 1, 1, PixelFormat.Rgb, PixelType.Float, ref info)
                .ReadBuffer(ReadBufferMode.None);

            UnbindAsReadBuffer(context);
        }

        public override bool IsBinded(IRenderingContext context)
        {
            context.GetInteger(GetPName.FramebufferBinding, out int handle);
            return handle == Handle;
        }

        internal void ReadPixel(IRenderingContext context,int x, int y, ref PixelInfo[ ] info)
        {
            BindAsReadBuffer(context);
            IRenderingContext.Current
               .ReadBuffer(ReadBufferMode.ColorAttachment0)
               .ReadPixel(x, y, 1, 1, PixelFormat.Rgb, PixelType.Float, info)
               .ReadBuffer(ReadBufferMode.None);
            UnbindAsReadBuffer(context);
        }
    }
}
