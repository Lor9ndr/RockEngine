using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

using RockEngine.Common.Utils;

namespace RockEngine.Common
{
    public class EngineWindow : GameWindow
    {

        #region BorderLessParams

        internal int minWidth = 800;
        internal int minHeight = 600;

        internal int maxWidth = 1920;
        internal int maxHeight = 1080;

        #endregion

        public unsafe new Vector2i Size
        {
            get
            {
                GLFW.GetWindowSize(WindowPtr, out var width, out var height);
                return new Vector2i(width, height);
            }
            set
            {
                var size = Vector2i.Clamp(value, new Vector2i(minWidth, minHeight), new Vector2i(maxWidth, maxHeight));
                GLFW.SetWindowSize(WindowPtr, size.X, size.Y);
            }
        }
        public event Func<Task> Initilized;

        public float DeltaTime;
        public int FPS;

        public double Time;
        private double oldTimeSinceStart;
        private int _framesPerSecond;
        private float _lastTime;

        public EngineWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override async void OnLoad()
        {
            base.OnLoad();

            GL.DebugMessageCallback(Logger.DebugProcCallback, nint.Zero);
            GL.Khr.DebugMessageCallback(Logger.DebugMessageDelegate, nint.Zero);
            GL.Arb.DebugMessageCallback(Logger.DebugMessageDelegateARB, nint.Zero);
            GL.Enable(EnableCap.DebugOutput);
            GL.Enable(EnableCap.DebugOutputSynchronous);
            await Initilized?.Invoke();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            CalculateFPS();
            Time += args.Time;
            DeltaTime = (float)Time - (float)oldTimeSinceStart;
            oldTimeSinceStart = Time;
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            SwapBuffers();
        }

        private void CalculateFPS()
        {
            float currentTime = (float)Time;

            _framesPerSecond++;
            if(currentTime - _lastTime > 1.0f)
            {
                _lastTime = currentTime;

                FPS = _framesPerSecond;

                _framesPerSecond = 0;
            }
        }
    }
}
