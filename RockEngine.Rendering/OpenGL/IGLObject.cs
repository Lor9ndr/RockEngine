namespace RockEngine.Rendering.OpenGL
{
    public interface IGLObject : IDisposable
    {
        public const int EMPTY_HANDLE = 0;
        public int Handle { get; }

        public IGLObject Bind(IRenderingContext context);

        public IGLObject Unbind(IRenderingContext context);

        public IGLObject SetLabel(IRenderingContext context);

        public bool IsBinded(IRenderingContext context);
        public void BindIfNotBinded(IRenderingContext context);

    }
}
