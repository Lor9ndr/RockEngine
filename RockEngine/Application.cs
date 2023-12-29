using OpenTK.Windowing.Common;

using RockEngine.DI;
using RockEngine.Common;
using RockEngine.Common.Utils;
using RockEngine.ECS.Layers;

namespace RockEngine
{
    public abstract class Application
    {
        private static Application? _instance;
        public static Application? GetCurrentApp() => _instance;
        public static EngineWindow? GetMainWindow() => _instance?.MainWindow;

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
            MainWindow = WindowManager.CreateWindow(name, width, height);
            MainWindow.RenderFrame += Render;
            MainWindow.UpdateFrame += Update;
            MainWindow.Load += Load;
            Layers = new Layers();
        }

        public virtual void Start()
        {
            MainWindow.Run();
        }

        protected abstract void Load();
        protected abstract void Update(FrameEventArgs args);
        protected abstract void Render(FrameEventArgs args);
    }
}
