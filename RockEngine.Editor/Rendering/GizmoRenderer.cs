using ImGuiNET;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

using RockEngine.Assets;
using RockEngine.Editor.GameObjects;
using RockEngine.Editor.Layers;
using RockEngine.Engine.ECS;
using RockEngine.Engine.ECS.GameObjects;
using RockEngine.Figures;
using RockEngine.Inputs;
using RockEngine.OpenGL;
using RockEngine.OpenGL.Shaders;
using RockEngine.OpenGL.Vertices;
using RockEngine.Rendering.Layers;
using RockEngine.Rendering.Renderers;


namespace RockEngine.Editor.Rendering
{
    public class GizmoRenderer : IRenderer, IDisposable
    {
        private readonly AShaderProgram _selectingShader;
        private readonly Vertex3D[] _axisVertices =
        new Vertex3D[ ]
        {
            // X-axis
            new Vertex3D(new Vector3(0.0f, 0.0f, 0.0f)),
            new Vertex3D(new Vector3(1.0f, 0.0f, 0.0f)),

            // Y-axis
            new Vertex3D(new Vector3(0.0f, 0.0f, 0.0f)),
            new Vertex3D(new Vector3(0.0f, 1.0f, 0.0f)),

            // Z-axis
            new Vertex3D(new Vector3(0.0f, 0.0f, 0.0f)),
            new Vertex3D(new Vector3(0.0f, 0.0f, 1.0f))
        };

        // Define the indices for the axes
        private readonly int[ ] _axisIndices =
        new int[ ]
        {
            0, 1, // X-axis
            2, 3, // Y-axis
            4, 5  // Z-axis
        };

        private readonly Mesh _axisMesh;
        private readonly GameObject _gizmoGameObject;
        private readonly CameraTexture _screen;
        private readonly PickingRenderer _pickingRenderer;
        private readonly Dictionary<int, Axis> _axes;
        private PickingTexture.PixelInfo CurrentPixelInfo;
        private Vector2 _lastMousePos;
        private bool _isDragging;
        private Axis? _currentAxis;
        private Transform _currentTransform;

        private event Action<Axis, Transform> StartDrag;

        public GizmoRenderer(CameraTexture screen)
        {
            var baseDebug = Path.Combine(PathConstants.RESOURCES, PathConstants.SHADERS, PathConstants.DEBUG, PathConstants.SELECTED_OBJECT);
            _selectingShader = ShaderProgram.GetOrCreate("SelectingObjectShader",
                new VertexShader(Path.Combine(baseDebug, "Selected.vert")),
                 new FragmentShader(Path.Combine(baseDebug, "Selected.frag")));

            _axisMesh = new Mesh(ref _axisVertices, ref _axisIndices, "GIZMO MESH AXIS", PathInfo.ENGINE_DIRECTORY, Guid.Empty);
            _axisMesh.SetupMeshIndicesVertices();
            _axisMesh.PrimitiveType = PrimitiveType.Lines;
            _gizmoGameObject = new GameObject("GIZMO", new MeshComponent(_axisMesh));

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
                var lineWidth = (DebugCamera.ActiveDebugCamera.Parent.Transform.Position - tr.Position).Length/2f;
                GL.LineWidth(lineWidth);
                _currentTransform = tr;
                // Setting transform data
                _gizmoGameObject.Transform.Position = tr.Position;
                _gizmoGameObject.Transform.Scale = new Vector3(Math.Max(tr.Scale.X, Math.Max(tr.Scale.Y, tr.Scale.Z))) * 5f;
                _gizmoGameObject.Transform.RotationQuaternion = Quaternion.Identity;

                // Picking pass
                _pickingRenderer.Begin();
                _pickingRenderer.ResizeTexture(_screen.ScreenTexture.Size);
                _gizmoGameObject.Update();
                _pickingRenderer.Render(_gizmoGameObject);
                _pickingRenderer.End();
                
                // DefaultRender pass to render gizmos
                _selectingShader.BindIfNotBinded();
                HandleClickingOnAxis();
                _gizmoGameObject.Update();
                _gizmoGameObject.Render();
                _selectingShader.SetShaderData("outlineColor", Vector3.One);
                _selectingShader.Unbind();
                GL.LineWidth(1);

                if(_isDragging)
                {
                    StartDrag.Invoke(_currentAxis, _currentTransform);
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
            CurrentPixelInfo = _pickingRenderer.GetPixelData((int)ImGuiRenderer.EditorScreenMousePos.X, (int)ImGuiRenderer.EditorScreenMousePos.Y);
            return Input.IsButtonDown(MouseButton.Left) && CurrentPixelInfo.PrimID >= 1 && CurrentPixelInfo.PrimID <= 3;
        }

        private void HandleClickingOnAxis()
        {
            if(_axes.TryGetValue((int)CurrentPixelInfo.PrimID, out var axis))
            {
                _selectingShader.SetShaderData("outlineColor", axis.Color);
            };
        }

        public void Dispose()
        {
            _axisMesh.Dispose();
        }
    }
}
