using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.Common.Utils;
using RockEngine.Rendering.OpenGL.Settings;

namespace RockEngine.Rendering.OpenGL.Textures
{
    public class Texture : BaseTexture
    {
        private Vector2i _size;

        public virtual Vector2i Size
        {
            get => _size;
            internal set => IRenderingContext.Update(context => Resize(context, value));
        }

        public override bool IsSetupped => Handle != IGLObject.EMPTY_HANDLE;

        public Texture(Vector2i size, TextureSettings settings)
            : base(settings)
        {
            size = new Vector2i(Math.Clamp(size.X, 1, int.MaxValue), Math.Clamp(size.Y, 1, int.MaxValue));
            _size = size;
        }

        public Texture()
            : base(TextureSettings.DefaultSettings)
        {
        }

        public virtual Texture SetTextureUnit(IRenderingContext context,int unit)
        {
            context.BindTextureUnit(unit, Handle);
            return this;
        }

        public override Texture SetLabel(IRenderingContext context)
        {
            string label = $"Texture: ({Handle})";
            Logger.AddLog($"Setupped {label}");

            context.ObjectLabel(ObjectLabelIdentifier.Texture, Handle, label.Length, label);
            return this;
        }

        public override Texture Setup(IRenderingContext context)
        {
            context.CreateTextures(Settings.TextureTarget,  out int handle);
            Handle = handle;
            if(Settings.IsMultisampled)
            {
                context.TextureStorage2DMultisample(Handle, Settings.SamplesCounter, Settings.SizedInternalFormat, Size, true);
            }
            else
            {
                context.TextureStorage2D(Handle, 1, Settings.SizedInternalFormat, Size);
            }
            SetTextureParameters(context);
            return this;
        }

        public virtual Texture Resize(IRenderingContext context, Vector2i size)
        {
            size = new Vector2i(Math.Clamp(size.X, 1, int.MaxValue), Math.Clamp(size.Y, 1, int.MaxValue));

            if(IsSetupped && _size != size)
            {
                _size = size;
                Dispose();
                IRenderingContext.Update(context =>
                {
                    Setup(context)
                        .SetLabel(context);
                    Logger.AddLog($"Resized Texture ({Handle}) with new size: {Size}");
                });
            }
            _size = size;

            return this;
        }

        public Texture SetTextureParameters(IRenderingContext context)
        {
            context.SetTextureParameters(this);
            return this;
        }
        public override Texture Bind(IRenderingContext context)
        {
            context.Bind(Settings.TextureTarget, Handle);
            return this;
        }
        public override Texture Unbind(IRenderingContext context)
        {
            context.Bind(Settings.TextureTarget, IGLObject.EMPTY_HANDLE);
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
                context.GetObjectLabel(ObjectLabelIdentifier.Texture, Handle, 64, out int length, out string name);
                if(length == 0)
                {
                    name = $"Texture: ({Handle})";
                }
                Logger.AddLog($"Disposing {name}");
                context.DeleteTexture(Handle);
                Handle = IGLObject.EMPTY_HANDLE;
            });
        }

        ~Texture()
        {
            Dispose();
        }

        public override bool IsBinded(IRenderingContext context)
        {
            context.GetInteger((GetPName)Settings.TextureTarget, out int value);
            return value == Handle;
        }
    }
}
