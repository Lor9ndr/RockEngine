﻿namespace RockEngine.Rendering.OpenGL
{
    public abstract class ASetuppableGLObject : ISetuppable, IGLObject
    {
        protected bool _disposed = false;

        protected int _handle;
        public abstract bool IsSetupped { get; }
        public int Handle { get => _handle; protected set => _handle = value; }
        public abstract IGLObject Bind(IRenderingContext context);
        public abstract IGLObject Unbind(IRenderingContext context);
        public abstract ISetuppable Setup(IRenderingContext context);
        public abstract IGLObject SetLabel(IRenderingContext context);
        public abstract void Dispose(bool disposing, IRenderingContext context = null);
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public abstract bool IsBinded(IRenderingContext context);
        public void BindIfNotBinded(IRenderingContext context)
        {
            if(!IsBinded(context))
            {
                Bind(context);
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
        public abstract IGLObject Bind(IRenderingContext context);
        public abstract IGLObject Unbind(IRenderingContext context);
        public abstract ISetuppable Setup(IRenderingContext context);
        public abstract IGLObject SetLabel(IRenderingContext context);
        public abstract void Dispose(bool disposing, IRenderingContext context = null);
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public abstract bool IsBinded(IRenderingContext context);
        public void BindIfNotBinded(IRenderingContext context)
        {
            if(!IsBinded(context))
            {
                Bind(context);
            }
        }
    }
}
