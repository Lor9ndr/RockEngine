using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace RockEngine.Utils
{
    public static class WindowManager
    {
        private static EngineWindow? _mainWindow;
        private static readonly List<EngineWindow> _windows = new List<EngineWindow>();

        public static EngineWindow GetMainWindow()
        {
            if (_mainWindow is null)
            {
                throw new NullReferenceException("MainWindow is not created");
            }
            return _mainWindow;
        }

        public static EngineWindow CreateWindow(string title, int width, int height)
        {
            var nativeSettings = NativeWindowSettings.Default;
            nativeSettings.APIVersion = new Version(4, 6);
#if DEBUG
            nativeSettings.Flags = ContextFlags.Debug;
#endif
            EngineWindow window;
            if (_mainWindow is not null)
            {
                nativeSettings.SharedContext = _mainWindow.Context;
                window = new EngineWindow(GameWindowSettings.Default, nativeSettings);

            }
            else
            {
                _mainWindow = window = new EngineWindow(GameWindowSettings.Default, nativeSettings);
            }

            window.Title = title;
            window.Size = new Vector2i(width, height);
            _windows.Add(window);
            window.Closing += (s) =>
            {
                _windows.Remove(window);
            };

            return window;
        }
    }
}
