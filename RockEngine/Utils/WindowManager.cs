using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace RockEngine.Utils
{
    public static class WindowManager
    {
        private static EngineWindow? _mainWindow;
        private static readonly List<EngineWindow> _windows = new List<EngineWindow>();

        static WindowManager()
        {
            GLFW.Init();
        }

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

        private static unsafe void CreateEmptyWindow(string title, int width, int height, EngineEmptyWindow engineWindow)
        {
            // Create a GLFW window
            GLFW.WindowHint(WindowHintClientApi.ClientApi, ClientApi.OpenGlApi);
            GLFW.WindowHint(WindowHintContextApi.ContextCreationApi, ContextApi.NativeContextApi);
            GLFW.WindowHint(WindowHintBool.Visible, true);
            GLFW.WindowHint(WindowHintBool.Focused, true);

            Window* window = GLFW.CreateWindow(width, height, title, null, null);
            if(window == null)
            {
                throw new Exception("Failed to create GLFW window.");
            }

            // Make the OpenGL context current
            GLFW.MakeContextCurrent(window);
            GL.LoadBindings(new GLFWBindingsContext());
            // Set up OpenGL settings
            GL.ClearColor(Color4.AliceBlue);

            // Run the window loop until it is closed
            while(!engineWindow.Exists)
            {
                // Clear the screen
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                engineWindow.Update();

                engineWindow.Render();

                // Swap the front and back buffers
                GLFW.SwapBuffers(window);

                // Poll for events
                GLFW.PollEvents();
                engineWindow.Exists = GLFW.WindowShouldClose(window);
            }
            engineWindow.Closing();

            // Destroy the GLFW window
            GLFW.DestroyWindow(window);
        }

        public static EngineEmptyWindow CreateWindowInThread(string title, int width, int height, out Task windowThread)
        {
            var engineWindow = new EngineEmptyWindow();
            // Start a new thread to run the window loop
            windowThread = Task.Run(() =>
            {
                CreateEmptyWindow(title, width, height, engineWindow);
            });

            return engineWindow;
        }
    }
}
