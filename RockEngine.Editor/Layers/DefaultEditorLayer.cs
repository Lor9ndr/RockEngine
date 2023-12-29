﻿using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RockEngine.Inputs;
using RockEngine.Rendering.Layers;
using RockEngine.Editor.GameObjects;
using RockEngine.DI;
using RockEngine.Physics;
using RockEngine.Editor.Rendering.Gizmo;
using RockEngine.ECS.GameObjects;
using RockEngine.Rendering.OpenGL;
using RockEngine.ECS;
using RockEngine.Common;
using RockEngine.Rendering.OpenGL.Shaders;
using RockEngine.Common.Utils;
using RockEngine.ECS.Layers;
using RockEngine.Rendering.OpenGL.Buffers.UBOBuffers;

namespace RockEngine.Editor.Layers
{
    public sealed class DefaultEditorLayer : ALayer, IDisposable
    {
        public CameraTexture Screen;
        public PickingTexture PickingTexture;
        public GameObject DebugCamera;
        public override int Order => 1;

        private readonly EngineWindow _window;
        private readonly PhysicsManager _physicsManager;
        private readonly AShaderProgram PickingShader;
        private readonly AShaderProgram SelectingShader;
        private readonly ImGuiRenderer _imguiRenderer;
        private readonly GizmoRenderer _gizmoRenderer;

        public DefaultEditorLayer()
        {
            _window = WindowManager.GetMainWindow();
            Screen = new CameraTexture(WindowManager.GetMainWindow().Size);
            PickingTexture = new PickingTexture(WindowManager.GetMainWindow().Size);

            var basePicking = Path.Combine(PathConstants.RESOURCES, PathConstants.SHADERS, PathConstants.DEBUG, PathConstants.PICKING);
            var baseDebug = Path.Combine(PathConstants.RESOURCES, PathConstants.SHADERS, PathConstants.DEBUG, PathConstants.SELECTED_OBJECT);

            PickingShader = ShaderProgram.GetOrCreate("PickingShader",
                new VertexShader(Path.Combine(basePicking, "Picking.vert")),
              new FragmentShader(Path.Combine(basePicking, "Picking.frag")));

            SelectingShader = ShaderProgram.GetOrCreate("SelectingObjectShader",
                new VertexShader(Path.Combine(baseDebug, "Selected.vert")),
                new FragmentShader(Path.Combine(baseDebug, "Selected.frag")));

            var camera = new DebugCamera(_window.Size.X / (float)_window.Size.Y, _window);

            DebugCamera = new GameObject("DebugCamera", camera, new Transform(new Vector3(0, 10, 0)));
            camera.LookAt(new Vector3(15), new Vector3(0), Vector3.UnitY);
            DebugCamera.OnStart();
            _physicsManager = IoC.Get<PhysicsManager>();
            _physicsManager.SetDebugRender(camera);
            _imguiRenderer = new ImGuiRenderer(Application.GetMainWindow()!,this, _physicsManager);
            _gizmoRenderer = new GizmoRenderer(Screen);
        }

        public override void OnRender(Scene scene)
        {
            DebugCamera.Update();
            DebugCamera.Render();
            PickingObjectPass(scene);

            MainRenderPass(scene);

            GettingObjectFromPicked(scene);
            _imguiRenderer.OnRender();
        }

        private void GettingObjectFromPicked(IEnumerable<GameObject> gameObjects)
        {
            if(Input.IsButtonPressed(MouseButton.Left) &&
                ImGuiRenderer.IsMouseOnEditorScreen)
            {
                var clickingOnAxis = _gizmoRenderer.IsClickingOnAxis();
                if(clickingOnAxis)
                {
                    return;
                }
                PixelInfo pi = new PixelInfo();
                PickingTexture.ReadPixel((int)ImGuiRenderer.EditorScreenMousePos.X, (int)ImGuiRenderer.EditorScreenMousePos.Y, ref pi);
                GameObject? selected = null;
                if((uint)pi.PrimID != 0 )
                {
                    var objID = (uint)pi.ObjectID;
                    selected = gameObjects.FirstOrDefault(s => s.GameObjectID == objID);
                }
                
                _imguiRenderer.SelectedGameObject = selected;
            }
        }

        private void MainRenderPass(Scene scene)
        {
            Screen.BeginRenderToScreen();
            var selected = _imguiRenderer.SelectedGameObject;
            if(selected != null)
            {
                selected.IsActive = false;
            }
            scene.EditorLayerRender();
            if(selected != null)
            {
                GL.Enable(EnableCap.Blend);
                OutlineSelectedGameObject(selected);
            }
            if(EditorSettings.DrawCollisions)
            {
               // _physicsManager.DebugRenderer.DebugRender();
            }

            Screen.EndRenderToScreen();
           
        }

        private void OutlineSelectedGameObject(GameObject selected)
        {
            selected.IsActive = true;
            GL.Clear(ClearBufferMask.StencilBufferBit);
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.StencilTest);

            GL.StencilFunc(StencilFunction.Always, 1, 0xFF);
            GL.StencilMask(0xFF);

            selected.Render();

            GL.StencilFunc(StencilFunction.Notequal, 1, 0xff);
            GL.StencilMask(0xFF);
            GL.Disable(EnableCap.DepthTest);

            SelectingShader.Bind();
            DebugCamera.Render();
            selected.Render();
            SelectingShader.Unbind();

            GL.StencilMask(0xFF);
            GL.StencilFunc(StencilFunction.Always, 0, 0xFF);
            GL.Enable(EnableCap.DepthTest);
            _gizmoRenderer.Render(selected.Transform);
        }

        private void PickingObjectPass(IEnumerable<GameObject> gameObjects)
        {
            PickingTexture.CheckSize(Screen.ScreenTexture.Size);

            PickingShader.Bind();
            DebugCamera.Render();
            GL.Disable(EnableCap.Blend);
            PickingTexture.BeginWrite();

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
                pd.SendData();
                gameObject.RenderMeshWithoutMaterial();

            }

            PickingTexture.EndWrite();
            PickingShader.Unbind();
            GL.Enable(EnableCap.Blend);
            GL.ClearColor(EditorSettings.BackGroundColor);
        }

        public void Dispose()
        {
            Screen.Dispose();
        }
    }
}
