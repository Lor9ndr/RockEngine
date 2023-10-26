using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RockEngine.Inputs;
using RockEngine.Utils;

using RockEngine.Rendering.Layers;

namespace RockEngine.Engine.ECS.GameObjects
{
    internal sealed class DebugCamera : Camera
    {
        public const int MAX_CAM_SPEED = 150;

        private bool _canMove;
        private readonly EngineWindow _window;
        private readonly KeyboardState? _keyboard;
        public float CameraSpeed { get; private set; } = 12.0f;
        public float Sensitivity { get; } = 0.125f;

        public bool CanMove
        {
            get => _canMove;
            set
            {
                _canMove = value;
                _window.Cursor = _canMove && ImGuiLayer.IsMouseOnEditorScreen ? MouseCursor.Hand : MouseCursor.Default;
            }
        }
        public DebugCamera(float aspectRatio, EngineWindow window) : base(aspectRatio)
        {
            _keyboard = window.KeyboardState;
            _window = window;
            _window.MouseWheel += Window_MouseWheel;
            _window.MouseMove += Window_MouseMove;
            _window.MouseDown += Window_MouseDown;
            _window.MouseUp += _window_MouseUp;
            _window.Resize += _window_Resize;
        }

        private void _window_Resize(ResizeEventArgs obj)
        {
            AspectRatio = obj.Size.X / (float)obj.Size.Y;
        }

        private void _window_MouseUp(MouseButtonEventArgs obj)
        {
            CanMove = false;
        }

        private void Window_MouseWheel(MouseWheelEventArgs obj)
        {
            CameraSpeed += obj.OffsetY;
            CameraSpeed = Math.Clamp(CameraSpeed, 0, MAX_CAM_SPEED);
            Logger.AddLog($"Change mouse speed to {CameraSpeed}");
        }

        private void Window_MouseDown(MouseButtonEventArgs obj)
        {
            CanMove = true;
        }
        private void ChangePosition(Vector3 offset)
        {
            if (CanMove && ImGuiLayer.IsMouseOnEditorScreen)
            {
                Parent!.Transform.Position += offset;
            }
        }

        private void Window_MouseMove(MouseMoveEventArgs mouse)
        {
            if (CanMove && ImGuiLayer.IsMouseOnEditorScreen)
            {
                Yaw += mouse.DeltaX * Sensitivity;
                Pitch -= mouse.DeltaY * Sensitivity;
            }
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            if(Input.IsKeyDown(Keys.W))
            {
                ChangePosition(Front * CameraSpeed * Time.DeltaTime); // Front
            }
            if(Input.IsKeyDown(Keys.S))
            {
                ChangePosition(-Front * CameraSpeed * Time.DeltaTime); // Front
            }
            if(Input.IsKeyDown(Keys.A))
            {
                ChangePosition(-Right * CameraSpeed * Time.DeltaTime); // Front
            }
            if(Input.IsKeyDown(Keys.D))
            {
                ChangePosition(Right * CameraSpeed * Time.DeltaTime); // Front
            }
            if(Input.IsKeyDown(Keys.Space))
            {
                ChangePosition(Up * CameraSpeed * Time.DeltaTime); // Front
            }
            if(Input.IsKeyDown(Keys.LeftShift))
            {
                ChangePosition(-Up * CameraSpeed * Time.DeltaTime); // Front
            }
        }

        public override void OnUpdateDevelepmentState()
        {
            //base.OnUpdateDevelepmentState();
            OnUpdate();
        }
        public override void RenderOnEditorLayer()
        {
            base.Render();
        }
    }
}
