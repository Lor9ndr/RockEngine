
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

using RockEngine.Common;
using RockEngine.Common.Utils;
using RockEngine.Common.Vertices;
using RockEngine.ECS;
using RockEngine.ECS.GameObjects;
using RockEngine.Editor.GameObjects;
using RockEngine.Editor.Layers;
using RockEngine.Inputs;
using RockEngine.Rendering;
using RockEngine.Rendering.Layers;
using RockEngine.Rendering.OpenGL.Shaders;
using RockEngine.Rendering.Renderers;

namespace RockEngine.Editor.Rendering.Gizmo
{
    public class GizmoRenderer : IRenderer<Transform>, IDisposable
    {
        private readonly AShaderProgram _gizmoShader;
        private readonly Vertex3D[ ] _axisVertices =
        [
            // X-axis
            new Vertex3D(Vector3.Zero, Vector3.UnitX),
            new Vertex3D(Vector3.UnitX, Vector3.UnitX),

            // Y-axis
            new Vertex3D(Vector3.Zero, Vector3.UnitY),
            new Vertex3D(Vector3.UnitY, Vector3.UnitY),

            // Z-axis
            new Vertex3D(Vector3.Zero, Vector3.UnitZ),
            new Vertex3D(Vector3.UnitZ, Vector3.UnitZ)
        ];

        // Define the indices for the axes
        private readonly int[ ] _axisIndices =
        [
            0, 1, // X-axis
            2, 3, // Y-axis
            4, 5  // Z-axis
        ];

        private readonly GameObject _gizmoPosGameObject;
        private readonly GameObject _gizmoRotGameObject;
        private readonly GameObject _gizmoScaleGameObject;
        private readonly CameraTexture _screen;
        private readonly PickingRenderer _pickingRenderer;
        private readonly Dictionary<int, Axis> _axes;
        private readonly DefaultEditorLayer _editorLayer;
        private PixelInfo CurrentPixelInfo;
        private Vector2 _lastMousePos;
        private bool _isDragging;
        private Axis? _currentAxis;
        private Transform _currentTransform;
        private GizmoMode _currentGizmoMode = GizmoMode.Position;

        private event Action<Axis, Transform> StartDrag;

        public GizmoRenderer(CameraTexture screen, DefaultEditorLayer editorLayer)
        {
            var baseDebug = new PathInfo(PathConstants.RESOURCES) / PathConstants.SHADERS / PathConstants.DEBUG / PathConstants.GIZMO;

            _gizmoShader = ShaderProgram.GetOrCreate("GizmoShader",
                new VertexShader(baseDebug / "Gizmo.vert"),
                 new FragmentShader(baseDebug / "Gizmo.frag"));
            IRenderingContext.Update(context =>
            {
                _gizmoShader.Setup(context);
            });

            var posMesh = new Mesh(ref _axisVertices, ref _axisIndices, "GIZMO MESH AXIS position", PathsInfo.ENGINE_DIRECTORY, Guid.Empty);
            posMesh.PrimitiveType = PrimitiveType.Lines;

            _gizmoPosGameObject = new GameObject("GIZMOPos", new MeshComponent(posMesh));
            _gizmoScaleGameObject = new GameObject("GIZMOScale", new MeshComponent(posMesh));
            _gizmoPosGameObject.OnStart();
            _gizmoScaleGameObject.OnStart();

            _screen = screen;
            _pickingRenderer = new PickingRenderer(_screen.ScreenTexture.Size);

            _axes = new Dictionary<int, Axis>()
            {
                { 1, new Axis("X", Vector3.UnitX, 1) },
                { 2, new Axis("Y", Vector3.UnitY, 2) },
                { 3, new Axis("Z", Vector3.UnitZ, 3) }
            };

            var window = Application.GetMainWindow()!;
            window.MouseDown += GizmoRenderer_MouseDown;
            window.MouseUp += GizmoRenderer_MouseUp;
            window.KeyDown += Window_KeyDown;
            StartDrag += GizmoRenderer_StartDrag;
            _editorLayer = editorLayer;
        }

        public void Render(IRenderingContext context, Transform item)
        {
            var camOffset = (DebugCamera.ActiveDebugCamera.Parent.Transform.Position - item.Position).Length;
            var lineLength = Math.Clamp(camOffset / 2, 2, 6);
            var lineWidth = Math.Clamp(lineLength, 8, 10);
            context.LineWidth(lineWidth);
            _currentTransform = item;
            // Setting transform data
            _gizmoPosGameObject.Transform.Position = item.Position;
            _gizmoPosGameObject.Transform.Scale = new Vector3(Math.Max(item.Scale.X, Math.Max(item.Scale.Y, item.Scale.Z))) * lineLength;
            _gizmoPosGameObject.Transform.RotationQuaternion = Quaternion.Identity;
            // Picking pass
            _gizmoPosGameObject.Update();

            var mesh = _gizmoPosGameObject.GetComponent<MeshComponent>()!.Mesh!;
            mesh.PrepareSendingModel(context, [_gizmoPosGameObject.Transform.GetModelMatrix()], 0, 1);
            _pickingRenderer.Begin(context);
            _pickingRenderer.ResizeTexture(_screen.ScreenTexture.Size);
            _pickingRenderer.Render(context, _gizmoPosGameObject);
            _pickingRenderer.End(context);
            // DefaultRender pass to render gizmos
            _gizmoShader.BindIfNotBinded(context);
            HandleClickingOnAxis(context);
            _gizmoShader.SetShaderData(context, "outlineColor", Vector3.One);
            _gizmoPosGameObject.Render(context);
            _gizmoShader.Unbind(context);
            context.LineWidth(1);

            if(_isDragging)
            {
                StartDrag.Invoke(_currentAxis!, _currentTransform);
            }
            _lastMousePos = (Vector2)ImGuiRenderer.EditorScreenMousePos;
        }

       

        public bool IsClickingOnAxis(IRenderingContext context)
        {
            _pickingRenderer.ReadPixel(context, (int)ImGuiRenderer.EditorScreenMousePos.X, (int)ImGuiRenderer.EditorScreenMousePos.Y, ref CurrentPixelInfo);

            return Input.IsButtonDown(MouseButton.Left) && CurrentPixelInfo.Blue >= 1 && CurrentPixelInfo.Blue <= 3;
        }

        private void HandleClickingOnAxis(IRenderingContext context)
        {
            if(_axes.TryGetValue((int)CurrentPixelInfo.Blue, out var axis))
            {
                _gizmoShader.SetShaderData(context, "outlineColor", axis.Color);
            };
        }

        private void Window_KeyDown(KeyboardKeyEventArgs e)
        {
            switch(e.Key)
            {
                case Keys.W:
                    _currentGizmoMode = GizmoMode.Position;
                    break;
                case Keys.E:
                    _currentGizmoMode = GizmoMode.Rotation;
                    break;
                case Keys.R:
                    _currentGizmoMode = GizmoMode.Scale;
                    break;
            }
        }
        private void GizmoRenderer_StartDrag(Axis axis, Transform transform)
        {
            var debugCam = _editorLayer.DebugCamera.GetComponent<DebugCamera>();
            var prevValue = debugCam.CanMove;
            if(_isDragging)
            {
                debugCam.CanMove = false;
                // Get the current mouse position
                Vector2 mousePos = (Vector2)ImGuiRenderer.EditorScreenMousePos;

                switch(_currentGizmoMode)
                {
                    case GizmoMode.Position:
                        DragAndSetPosition(mousePos, transform, axis.Color);
                        break;
                    case GizmoMode.Rotation:
                        DragAndSetRotation(mousePos, transform, axis.Color);
                        break;
                    case GizmoMode.Scale:
                        DragAndSetScale(mousePos, transform, axis.Color);

                        break;
                }
            }
            else
            {
                debugCam.CanMove = prevValue;
            }
        }

        public void DragAndSetPosition(Vector2 mousePosition, Transform transform, Vector3 directionAxis)
        {
            // Calculate the offset in the plane
            var delta = mousePosition - _lastMousePos;
            var offset = delta.X + delta.Y;

            // Update the position of the object being manipulated
            transform.Position += directionAxis * offset * 0.05f;

            // Update the last mouse position
            _lastMousePos = mousePosition;
        }
        private void DragAndSetScale(Vector2 mousePos, Transform transform, Vector3 axis)
        {
            var delta = mousePos - _lastMousePos;
            var offset = delta.X + delta.Y;

            // Update the position of the object being manipulated
            transform.Scale += axis * offset * 0.05f;

            // Update the last mouse position
            _lastMousePos = mousePos;
        }

        private void DragAndSetRotation(Vector2 mousePos, Transform transform, Vector3 axis)
        {
            var delta = mousePos - _lastMousePos;
            float rotationAmount = delta.X * 0.01f; // Adjust based on horizontal mouse movement

            // Create a quaternion rotation around the specified axis
            Quaternion rotationDelta = Quaternion.FromAxisAngle(axis, rotationAmount);
            transform.RotationQuaternion *= rotationDelta;

            _lastMousePos = mousePos;
        }

        private void GizmoRenderer_MouseUp(MouseButtonEventArgs obj)
        {
            _isDragging = false;
            _currentAxis = null;
        }

        private void GizmoRenderer_MouseDown(MouseButtonEventArgs obj)
        {
            IRenderingContext.Update(context =>
            {
                if(ImGuiRenderer.IsMouseOnEditorScreen && IsClickingOnAxis(context) &&
                _axes.TryGetValue((int)CurrentPixelInfo.Blue, out var axis) &&
                _currentTransform is not null)
                {
                    _isDragging = true;
                    _currentAxis = axis;
                }
            });

        }

        public void Dispose() => _gizmoPosGameObject.Dispose();

        public void Update(Transform item)
        {
            throw new NotImplementedException();
        }
    }
}
