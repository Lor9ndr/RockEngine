using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

using RockEngine.Utils;

namespace RockEngine
{
    public class EngineWindow : GameWindow
    {
        public static readonly Color4 BACK_GROUND_COLOR = new Color4(0.6f, 0.67f, 0.96f, 1);

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

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.DebugMessageCallback(Logger.DebugProcCallback, IntPtr.Zero);
            GL.Khr.DebugMessageCallback(Logger.DebugMessageDelegate, IntPtr.Zero);
            GL.Arb.DebugMessageCallback(Logger.DebugMessageDelegateARB, IntPtr.Zero);
            GL.Enable(EnableCap.DebugOutput);
            GL.Enable(EnableCap.DebugOutputSynchronous);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            CalculateFPS();
            Time += args.Time;
            DeltaTime = (float)Time - (float)oldTimeSinceStart;
            oldTimeSinceStart = Time;
        }

        protected unsafe override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            GL.ClearColor(BACK_GROUND_COLOR);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            base.OnRenderFrame(args);

            SwapBuffers();

        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
        }
        private void CalculateFPS()
        {
            float currentTime = (float)Time;

            _framesPerSecond++;
            if (currentTime - _lastTime > 1.0f)
            {
                _lastTime = currentTime;

                FPS = _framesPerSecond;

                _framesPerSecond = 0;
            }
        }
    }
}
