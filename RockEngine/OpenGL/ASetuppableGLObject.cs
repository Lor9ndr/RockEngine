namespace RockEngine.OpenGL
{
    public abstract class ASetuppableGLObject : ISetuppable, IGLObject
    {
        protected bool _disposed = false;

        protected int _handle;
        public abstract bool IsSetupped { get; }
        public int Handle { get => _handle; protected set => _handle = value; }
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
        public void BindIfNotBinded()
        {
            if(!IsBinded())
            {
                Bind();
            }
        }
    }

    public abstract class ASetuppableGLObject<TSettings> : ISetuppable<TSettings>, IGLObject where TSettings : ISettings
    {
        protected bool _disposed = false;
        public event Action OnSettingsChanged;

        protected int _handle;

        private TSettings _settings;
        public TSettings Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                OnSettingsChanged?.Invoke();
            }
        }
        public abstract bool IsSetupped { get; }
        public int Handle { get => _handle; protected set => _handle = value; }
        public ASetuppableGLObject(TSettings settings)
        {
            _settings = settings;
        }
        public ASetuppableGLObject() => Settings = default;
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
        public void BindIfNotBinded()
        {
            if(!IsBinded())
            {
                Bind();
            }
        }
    }
}
