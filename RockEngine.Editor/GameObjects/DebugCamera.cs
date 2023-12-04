using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RockEngine.Utils;

using RockEngine.Rendering.Layers;
using RockEngine.Engine.ECS.GameObjects;
using RockEngine.Inputs;
using RockEngine.Engine;

namespace RockEngine.Editor.GameObjects
{
    public sealed class DebugCamera : Camera
    {
        public const int MAX_CAM_SPEED = 150;

        private bool _canMove;
        private readonly EngineWindow _window;
        private readonly KeyboardState? _keyboard;
        public float CameraSpeed { get; private set; } = 12.0f;
        public float Sensitivity { get; } = 0.125f;

        public static DebugCamera ActiveDebugCamera;

        public bool CanMove
        {
            get => _canMove;
            set
            {
                _canMove = value && ImGuiRenderer.IsMouseOnEditorScreen;
                _window.Cursor = _canMove ? MouseCursor.Hand : MouseCursor.Default;
            }
        }
        public DebugCamera(float aspectRatio, EngineWindow window) : base(aspectRatio)
        {
            _keyboard = window.KeyboardState;
            _window = window;
            _window.MouseWheel += Window_MouseWheel;
            _window.MouseMove += Window_MouseMove;
            _window.MouseDown += Window_MouseDown;
            _window.MouseUp += Window_MouseUp;
            _window.Resize += Window_Resize;
            ActiveDebugCamera = this;
        }

        private void Window_Resize(ResizeEventArgs obj)
        {
            AspectRatio = obj.Size.X / (float)obj.Size.Y;
        }

        private void Window_MouseUp(MouseButtonEventArgs obj)
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
            if(CanMove)
            {
                Parent!.Transform.Position += offset;
            }
        }

        private void Window_MouseMove(MouseMoveEventArgs mouse)
        {
            if(CanMove)
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
    }
}
