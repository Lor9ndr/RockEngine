namespace RockEngine.Rendering.OpenGL
{
    public interface ISetuppable<TSettings> : ISetuppable where TSettings : ISettings
    {
        public TSettings Settings { get; }
    }

    public interface ISetuppable
    {
        public bool IsSetupped { get; }
        public ISetuppable Setup(IRenderingContext context);
    }

    public interface ISettings
    {
    }
}
