using OpenTK.Graphics.OpenGL4;

using RockEngine.Assets;
using RockEngine.OpenGL.Settings;

namespace RockEngine.OpenGL.Textures
{
    public abstract class BaseTexture : BaseAsset, ISetuppable<TextureSettings>, IGLObject
    {
        protected bool _disposed = false;
        public event Action OnSettingsChanged;

        private TextureSettings _settings;
        public TextureSettings Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                OnSettingsChanged?.Invoke();
            }
        }
        public abstract bool IsSetupped { get; }
        public abstract int Handle { get; protected set; }
        public BaseTexture()
         : this(string.Empty, "Texture", Guid.Empty)
        {
        }

        public BaseTexture(string path, string name, Guid id)
            : base(path, name, id, AssetType.Texture)
        {
            Settings = default;
        }
        public BaseTexture(TextureSettings settings)
            : base(string.Empty, "Texture", Guid.Empty, AssetType.Texture)
        {
            _settings = settings;
        }

        public abstract IGLObject Bind();
        public abstract IGLObject Unbind();
        public abstract ISetuppable Setup();
        public abstract IGLObject SetLabel();
        protected abstract void Dispose(bool disposing);
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public abstract bool IsBinded();
    }
}
