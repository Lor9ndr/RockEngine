using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

using RockEngine.Common;
using RockEngine.Common.Utils;
using RockEngine.Common.Vertices;
using RockEngine.ECS;
using RockEngine.ECS.GameObjects;
using RockEngine.Editor.GameObjects;
using RockEngine.Editor.Layers;
using RockEngine.Inputs;
using RockEngine.Renderers;
using RockEngine.Rendering.Layers;
using RockEngine.Rendering.OpenGL.Shaders;

namespace RockEngine.Editor.Rendering.Gizmo
{
    public class GizmoRenderer : IRenderer, IDisposable
    {
        private readonly AShaderProgram _selectingShader;
        private readonly Vertex3D[ ] _axisVertices =
        new Vertex3D[ ]
        {
            // X-axis
            new Vertex3D(Vector3.Zero, Vector3.UnitX),
            new Vertex3D(Vector3.UnitX, Vector3.UnitX),

            // Y-axis
            new Vertex3D(Vector3.Zero, Vector3.UnitY),
            new Vertex3D(Vector3.UnitY, Vector3.UnitY),

            // Z-axis
            new Vertex3D(Vector3.Zero, Vector3.UnitZ),
            new Vertex3D(Vector3.UnitZ, Vector3.UnitZ)
        };

        // Define the indices for the axes
        private readonly int[ ] _axisIndices =
        new int[ ]
        {
            0, 1, // X-axis
            2, 3, // Y-axis
            4, 5  // Z-axis
        };

        private readonly GameObject _gizmoPosGameObject;
        private readonly GameObject _gizmoRotGameObject;
        private readonly GameObject _gizmoScaleGameObject;
        private readonly CameraTexture _screen;
        private readonly PickingRenderer _pickingRenderer;
        private readonly Dictionary<int, Axis> _axes;
        private PixelInfo CurrentPixelInfo;
        private Vector2 _lastMousePos;
        private bool _isDragging;
        private Axis? _currentAxis;
        private Transform _currentTransform;

        private event Action<Axis, Transform> StartDrag;

        public GizmoRenderer(CameraTexture screen)
        {
            var baseDebug = new PathInfo(PathConstants.RESOURCES) / PathConstants.SHADERS / PathConstants.DEBUG / PathConstants.GIZMO;

            _selectingShader = ShaderProgram.GetOrCreate("GizmoShader",
                new VertexShader(baseDebug / "Gizmo.vert"),
                 new FragmentShader(baseDebug / "Gizmo.frag"));
            _selectingShader.Setup();

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
            StartDrag += GizmoRenderer_StartDrag;
        }

        private void GizmoRenderer_StartDrag(Axis axis, Transform transform)
        {
            var editorLayer = Application.GetCurrentApp().Layers.GetLayer<DefaultEditorLayer>();
            var debugCam = editorLayer.DebugCamera.GetComponent<DebugCamera>();
            var prevValue = debugCam.CanMove;
            if(_isDragging)
            {
                debugCam.CanMove = false;
                // Get the current mouse position
                Vector2 mousePos = (Vector2)ImGuiRenderer.EditorScreenMousePos;

                // Drag and set the position based on the axis being dragged
                DragAndSetPosition(mousePos, transform, axis.Color);
            }
            else
            {
                debugCam.CanMove = prevValue;
            }
        }

        private void GizmoRenderer_MouseUp(OpenTK.Windowing.Common.MouseButtonEventArgs obj)
        {
            _isDragging = false;
            _currentAxis = null;
        }

        private void GizmoRenderer_MouseDown(OpenTK.Windowing.Common.MouseButtonEventArgs obj)
        {
            if(ImGuiRenderer.IsMouseOnEditorScreen && IsClickingOnAxis() &&
                _axes.TryGetValue((int)CurrentPixelInfo.PrimID, out var axis) &&
                _currentTransform is not null)
            {
                _isDragging = true;
                _currentAxis = axis;
            }
        }

        public void Render(GameObject go)
        {
            Render(go.Transform);
        }

        public void Render(IComponent component)
        {
            if(component is Transform tr)
            {
                var camOffset = (DebugCamera.ActiveDebugCamera.Parent.Transform.Position - tr.Position).Length;
                var lineLength = Math.Clamp(camOffset / 2, 2, 6);
                var lineWidth = Math.Clamp(lineLength, 8, 10);
                GL.LineWidth(lineWidth);
                _currentTransform = tr;
                // Setting transform data
                _gizmoPosGameObject.Transform.Position = tr.Position;
                _gizmoPosGameObject.Transform.Scale = new Vector3(Math.Max(tr.Scale.X, Math.Max(tr.Scale.Y, tr.Scale.Z))) * lineLength;
                _gizmoPosGameObject.Transform.RotationQuaternion = Quaternion.Identity;
                // Picking pass
                _pickingRenderer.Begin();
                _pickingRenderer.ResizeTexture(_screen.ScreenTexture.Size);
                _gizmoPosGameObject.Update();
                _pickingRenderer.Render(_gizmoPosGameObject);
                _pickingRenderer.End();

                // DefaultRender pass to render gizmos
                _selectingShader.BindIfNotBinded();
                HandleClickingOnAxis();
                _gizmoPosGameObject.Render();
                _selectingShader.SetShaderData("outlineColor", Vector3.One);
                _selectingShader.Unbind();
                GL.LineWidth(1);

                if(_isDragging)
                {
                    StartDrag.Invoke(_currentAxis!, _currentTransform);
                }
            }
            _lastMousePos = (Vector2)ImGuiRenderer.EditorScreenMousePos;
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

        public bool IsClickingOnAxis()
        {
            _pickingRenderer.ReadPixel((int)ImGuiRenderer.EditorScreenMousePos.X, (int)ImGuiRenderer.EditorScreenMousePos.Y, ref CurrentPixelInfo);

            return Input.IsButtonDown(MouseButton.Left) && CurrentPixelInfo.PrimID >= 1 && CurrentPixelInfo.PrimID <= 3;
        }

        private void HandleClickingOnAxis()
        {
            if(_axes.TryGetValue((int)CurrentPixelInfo.PrimID, out var axis))
            {
                _selectingShader.SetShaderData("outlineColor", axis.Color);
            };
        }

        public void Dispose() => _gizmoPosGameObject.Dispose();
    }
}
