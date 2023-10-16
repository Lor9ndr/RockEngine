namespace RockEngine.OpenGL
{
    public interface ISetuppable<TSettings> : ISetuppable where TSettings : ISettings
    {
        public TSettings Settings { get; }
    }

    public interface ISetuppable
    {
        public bool IsSetupped { get; }
        public ISetuppable Setup();
    }

    public interface ISettings
    {
    }
}
