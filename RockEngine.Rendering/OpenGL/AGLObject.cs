namespace RockEngine.Rendering.OpenGL
{
    public abstract class AGLObject : IGLObject
    {
        public const int EMPTY_HANDLE = 0;

        public int Handle { get; protected set; }

        protected bool _disposed;

        public abstract IGLObject Bind(IRenderingContext context);

        protected abstract void Dispose(bool disposing);
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public abstract IGLObject SetLabel(IRenderingContext context);

        public abstract IGLObject Unbind(IRenderingContext context);

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
