namespace RockEngine.OpenGL
{
    public interface IGLObject : IDisposable
    {
        public const int EMPTY_HANDLE = 0;
        public int Handle { get; }

        public IGLObject Bind();

        public IGLObject Unbind();

        public IGLObject SetLabel();

        public bool IsBinded();
        public void BindIfNotBinded()
        {
            if(!IsBinded())
            {
                Bind();
            }
        }
    }
}
