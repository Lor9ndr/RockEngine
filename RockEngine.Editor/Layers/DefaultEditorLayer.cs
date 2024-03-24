using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

using RockEngine.Common;
using RockEngine.Common.Utils;
using RockEngine.DI;
using RockEngine.ECS;
using RockEngine.ECS.GameObjects;
using RockEngine.ECS.Layers;
using RockEngine.Editor.GameObjects;
using RockEngine.Editor.Rendering.Gizmo;
using RockEngine.Inputs;
using RockEngine.Physics;
using RockEngine.Physics.Colliders;
using RockEngine.Rendering;
using RockEngine.Rendering.Layers;
using RockEngine.Rendering.OpenGL;
using RockEngine.Rendering.OpenGL.Buffers.UBOBuffers;
using RockEngine.Rendering.OpenGL.Shaders;

using System.Diagnostics;

namespace RockEngine.Editor.Layers
{
    public sealed class DefaultEditorLayer : ALayer, IDisposable
    {
        public CameraTexture Screen;
        public PickingTexture PickingTexture;
        public GameObject DebugCamera;
        public Stopwatch Watcher;

        public override int Order => 1;

        private readonly EngineWindow _window;
        private readonly PhysicsManager _physicsManager;
        private readonly AShaderProgram _pickingShader;
        private readonly AShaderProgram _selectingShader;
        private readonly ImGuiRenderer _imguiRenderer;
        private readonly GizmoRenderer _gizmoRenderer;

        public DefaultEditorLayer()
        {
            Watcher = new Stopwatch();
            _window = WindowManager.GetMainWindow();
            Screen = new CameraTexture(WindowManager.GetMainWindow().Size);
            PickingTexture = new PickingTexture(WindowManager.GetMainWindow().Size);

            var basePicking = Path.Combine(PathConstants.RESOURCES, PathConstants.SHADERS, PathConstants.DEBUG, PathConstants.PICKING);
            var baseDebug = Path.Combine(PathConstants.RESOURCES, PathConstants.SHADERS, PathConstants.DEBUG, PathConstants.SELECTED_OBJECT);

            _pickingShader = ShaderProgram.GetOrCreate("PickingShader",
              new VertexShader(Path.Combine(basePicking, "Picking.vert")),
            new FragmentShader(Path.Combine(basePicking, "Picking.frag")));

            _selectingShader = ShaderProgram.GetOrCreate("SelectingObjectShader",
                    new VertexShader(Path.Combine(baseDebug, "Selected.vert")),
                    new FragmentShader(Path.Combine(baseDebug, "Selected.frag")));

            IRenderingContext.Update(context =>
            {
                _pickingShader.Setup(context);
                _selectingShader.Setup(context);
            });
          

            var camera = new DebugCamera(_window.Size.X / (float)_window.Size.Y, _window);

            DebugCamera = new GameObject("DebugCamera", camera, new Transform(new Vector3(0, 10, 0)));
            camera.LookAt(new Vector3(15), new Vector3(0), Vector3.UnitY);
            DebugCamera.OnStart();
            _physicsManager = IoC.Get<PhysicsManager>();
            _physicsManager.SetDebugRender(camera);
            _imguiRenderer = new ImGuiRenderer(Application.GetMainWindow()!, this, _physicsManager);
            _gizmoRenderer = new GizmoRenderer(Screen);

            // First init to init UBO for that
            PickingData pd = new PickingData();
        }

        public override async Task OnRender(Scene scene)
        {
            if(EditorSettings.DrawCollisions)
            {
                //_physicsManager.World.DebugWorld.DrawBVH(Vector3.UnitX);

                foreach(var item in scene)
                {
                    var body = item.GetComponent<EngineRigidBody>();
                    if(body is not null && body.Collider is not null)
                    {
                      if(body.Collider.WasCollided)
                      {
                          _physicsManager.World.ColliderRenderer.DrawOBB((OBB)body.Collider, Vector4.UnitX);
                      }
                      else
                      {
                          _physicsManager.World.ColliderRenderer.DrawOBB((OBB)body.Collider, Vector4.UnitY);
                      }
                    }
                }
            }
            Watcher.Restart();
            DebugCamera.Update();
            IRenderingContext.Render(DebugCamera.Render);
            
            MainRenderPass(scene);

             PickingObjectPass(scene);
             GettingObjectFromPicked(scene);
            
            Watcher.Stop();
            await _imguiRenderer.OnRender().ConfigureAwait(false);
        }

        private void GettingObjectFromPicked(IEnumerable<GameObject> gameObjects)
        {
            if(Input.IsButtonPressed(MouseButton.Left) &&
                ImGuiRenderer.IsMouseOnEditorScreen)
            {
                IRenderingContext.Render(context =>
                {
                    var clickingOnAxis = _gizmoRenderer.IsClickingOnAxis(context);
                    if(clickingOnAxis)
                    {
                        return;
                    }
                    PixelInfo pi = new PixelInfo();
                    PickingTexture.ReadPixel(context, (int)ImGuiRenderer.EditorScreenMousePos.X, (int)ImGuiRenderer.EditorScreenMousePos.Y, ref pi);
                    GameObject? selected = null;
                    if((uint)pi.PrimID != 0)
                    {
                        var objID = (uint)pi.ObjectID;
                        selected = gameObjects.FirstOrDefault(s => s.GameObjectID == objID);
                    }

                    _imguiRenderer.SelectedGameObject = selected;
                });
               
            }
        }

        private void MainRenderPass(Scene scene)
        {
            IRenderingContext.Render(context =>
            {
                context.Enable(EnableCap.CullFace)
                .Enable(EnableCap.DepthTest);
                var selected = _imguiRenderer.SelectedGameObject;
                if(selected != null)
                {
                    selected.IsActive = false;
                }
                Screen.BeginRenderToScreen(context);

                scene.EditorLayerRender(context);
                if(selected != null)
                {
                    OutlineSelectedGameObject(context, selected);
                }
                if(EditorSettings.DrawCollisions)
                {
                    _physicsManager.World.ColliderRenderer.Render();
                }
                Screen.EndRenderToScreen(context);
            });
        }

        private void OutlineSelectedGameObject(IRenderingContext context, GameObject selected)
        {
            // Save initial states
            context.IsEnabled(EnableCap.StencilTest, out bool stencilTestEnabled);
            context.IsEnabled(EnableCap.DepthTest, out bool depthTestEnabled);

            // Ensure the selected object is active for rendering
            selected.IsActive = true;

            // Prepare stencil buffer to mark pixels where the selected object is drawn
            context.Enable(EnableCap.StencilTest);
            context.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);
            context.StencilFunc(StencilFunction.Always, 1, 0xFF);
            context.StencilMask(0xFF); // Enable writing to the stencil buffer
            context.Clear(ClearBufferMask.StencilBufferBit);

            // Render the selected object normally, marking its pixels in the stencil buffer
            selected.Render(context);

            // Render the outline where the stencil buffer is not equal to 1 (i.e., around the selected object)
            context.StencilFunc(StencilFunction.Notequal, 1, 0xFF);
            context.StencilMask(0x00); // Disable writing to the stencil buffer to keep the outline marks
            context.Disable(EnableCap.DepthTest); // Disable depth test to ensure the outline is always visible

            // Bind the shader for outlining and render the outline
            _selectingShader.Bind(context);
            DebugCamera.Render(context);
            selected.Render(context); // Render the selected object again or render a specific outline mesh if you have one
            _selectingShader.Unbind(context);

            // Restore the original stencil function and depth test state
            context.StencilFunc(StencilFunction.Always, 0, 0xFF);
            context.StencilMask(0xFF); // Re-enable writing to the stencil buffer if needed elsewhere

            if(depthTestEnabled)
            {
                context.Enable(EnableCap.DepthTest);
            }
            else
            {
                context.Disable(EnableCap.DepthTest);
            }

            if(!stencilTestEnabled)
            {
                context.Disable(EnableCap.StencilTest); // Restore original stencil test state
            }

            // Render gizmos or other overlays that should ignore the stencil buffer
            _gizmoRenderer.Render(context, selected.Transform);
        }

        private void PickingObjectPass(IEnumerable<GameObject> gameObjects)
        {
            IRenderingContext.Render(context =>
            {
                PickingTexture.CheckSize(Screen.ScreenTexture.Size);

                _pickingShader.Bind(context);
                DebugCamera.Render(context);
                context.Enable(EnableCap.Blend);
                PickingTexture.BeginWrite(context);

                for(int i = 0; i < gameObjects?.Count(); i++)
                {
                    GameObject? gameObject = gameObjects.ElementAt(i);
                    PickingData pd = new PickingData()
                    {
                        gObjectIndex = gameObject.GameObjectID,
                        gDrawIndex = (uint)i,
                    };
                    if(gameObject.GetComponent<Camera>() is not null)
                    {
                        continue;
                    }
                    pd.SendData(context);
                    gameObject.RenderMeshWithoutMaterial(context);
                }

                PickingTexture.EndWrite(context);
                _pickingShader.Unbind(context);
                context.Disable(EnableCap.Blend)
                    .ClearColor(EditorSettings.BackGroundColor);
            });
        }

        public void Dispose()
        {
            Screen.Dispose();
        }
    }
}
