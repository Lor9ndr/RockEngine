namespace RockEngine.OpenGL
{
    public abstract class AGLObject : IGLObject
    {
        public const int EMPTY_HANDLE = 0;

        public int Handle { get; protected set; }

        protected bool _disposed;

        public abstract IGLObject Bind();

        protected abstract void Dispose(bool disposing);
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public abstract IGLObject SetLabel();

        public abstract IGLObject Unbind();

        public abstract bool IsBinded();
    }
}
