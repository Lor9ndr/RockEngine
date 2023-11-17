using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.Assets;
using RockEngine.OpenGL.Settings;
using RockEngine.Utils;

namespace RockEngine.OpenGL.Textures
{
    public class Texture : BaseTexture
    {
        private Vector2i _size;

        public virtual Vector2i Size
        {
            get => _size;
            internal set => Resize(value);
        }

        public override bool IsSetupped => Handle != IGLObject.EMPTY_HANDLE;

        public override int Handle { get; protected set; }

        public override AssetType Type => AssetType.Texture;

        public Texture(Vector2i size, TextureSettings settings, Guid id, string path = "", string name = "Texture")
            : this(path, id, name, settings)
        {
            size = new Vector2i(Math.Clamp(size.X, 1, int.MaxValue), Math.Clamp(size.Y, 1, int.MaxValue));
            _size = size;
        }

        public Texture(string path, Guid id, string name = "Texture", TextureSettings settings = default)
            : base(settings)
        {
            ID = id;
            Path = path;
            Name = name;
            /* OnSettingsChanged += () =>
             {
                 Dispose();
                 Setup();
             };*/
        }

        public Texture(Vector2i size, string name, TextureSettings settings)
            : base(settings)
        {
            Size = size;
            Name = name;
            Path = PathInfo.ENGINE_DIRECTORY;
            /* OnSettingsChanged += () =>
             {
                 Dispose();
                 Setup();
             };*/
        }
        public Texture(Vector2i size, TextureSettings settings)
            : base(settings)
        {
            Size = size;
            Name = "Texture";
            Path = PathInfo.ENGINE_DIRECTORY;
            /*  OnSettingsChanged += () =>
              {
                  Dispose();
                  Setup();
              };*/
        }
        public Texture()
            : base()
        {
            Settings = TextureSettings.DefaultSettings;
            Path = string.Empty;
            ID = Guid.NewGuid();
            Name = "Texture";
            /* OnSettingsChanged += () =>
             {
                 Dispose();
                 Setup();
             };*/
        }

        public virtual Texture SetTextureUnit(int unit)
        {
            GL.BindTextureUnit(unit, Handle);
            return this;
        }

        public override Texture SetLabel()
        {
            string label = $"Texture: ({Handle})";
            Logger.AddLog($"Setupped {label}");

            GL.ObjectLabel(ObjectLabelIdentifier.Texture, Handle, label.Length, label);
            return this;
        }

        public override Texture Setup()
        {
            GL.CreateTextures(Settings.TextureTarget, 1, out int handle);
            Handle = handle;
            if (Settings.IsMultisampled)
            {
                GL.TextureStorage2DMultisample(Handle, Settings.SamplesCounter, Settings.SizedInternalFormat, Size.X, Size.Y, true);
            }
            else
            {
                GL.TextureStorage2D(Handle, 1, Settings.SizedInternalFormat, Size.X, Size.Y);
            }
            return this;
        }

        public virtual Texture Resize(Vector2i size)
        {
            size = new Vector2i(Math.Clamp(size.X, 1, int.MaxValue), Math.Clamp(size.Y, 1, int.MaxValue));

            if (IsSetupped && _size != size)
            {
                _size = size;
                Dispose();
                Setup()
                    .SetLabel();
                Logger.AddLog($"Resized Texture ({Handle}) with new size: {Size}");
            }
            _size = size;

            return this;
        }

        public Texture SetTextureParameters()
        {
            if (Settings.TextureParameters is not null)
            {
                foreach (var param in Settings.TextureParameters)
                {
                    GL.TextureParameter(Handle, param.Key, param.Value);
                }
            }
            return this;
        }
        public override Texture Bind()
        {
            GL.BindTexture(Settings.TextureTarget, Handle);
            return this;
        }
        public override Texture Unbind()
        {
            GL.BindTexture(Settings.TextureTarget, IGLObject.EMPTY_HANDLE);
            return this;
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                // Освободите управляемые ресурсы здесь
            }

            if (!IsSetupped)
            {
                return;
            }
            GL.GetObjectLabel(ObjectLabelIdentifier.Texture, Handle, 64, out int length, out string name);
            if (length == 0)
            {
                name = $"Texture: ({Handle})";
            }
            Logger.AddLog($"Disposing {name}");
            GL.DeleteTexture(Handle);
            Handle = IGLObject.EMPTY_HANDLE;
        }

        ~Texture()
        {
            Dispose();
        }

        public override bool IsBinded()
           => GL.GetInteger((GetPName)Settings.TextureTarget) == Handle;
    }
}
