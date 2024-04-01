using OpenTK.Windowing.Common;

using RockEngine.Common;
using RockEngine.Common.Utils;
using RockEngine.DI;
using RockEngine.ECS.Layers;

namespace RockEngine
{
    public abstract class Application : IDisposable
    {
        private static Application? _instance;
        public static Application? GetCurrentApp() => _instance;
        public static EngineWindow? GetMainWindow() => _instance?.MainWindow;
        protected CancellationTokenSource _ctr;
        protected CancellationToken _ct;

        public EngineWindow MainWindow;
        public readonly Layers Layers;
        public Application(string name, int width = 1280, int height = 720)
        {
            IoC.Setup();
            if(_instance != null)
            {
                throw new Exception("Application is already created");
            }
            else
            {
                _instance = this;
            }
            _ctr = new CancellationTokenSource();
            _ct = _ctr.Token;

            MainWindow = WindowManager.CreateWindow(name, width, height);
            MainWindow.RenderFrame += Render;
            MainWindow.UpdateFrame += Update;
            MainWindow.Initilized += InitilizedAsync;

            Layers = new Layers();
        }

        public virtual void Start()
        {
            MainWindow.Run();
        }
        protected abstract Task InitilizedAsync();
        protected abstract void Update(FrameEventArgs args);
        protected abstract void Render(FrameEventArgs args);

        public virtual void Dispose()
        {
            _ctr.Cancel();
            _ctr.Dispose();
        }
    }
}
