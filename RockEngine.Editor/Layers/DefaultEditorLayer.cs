﻿using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

using RockEngine.Assets;
using RockEngine.Engine;
using RockEngine.Engine.ECS.GameObjects;
using RockEngine.Inputs;
using RockEngine.OpenGL;
using RockEngine.OpenGL.Buffers.UBOBuffers;
using RockEngine.OpenGL.Shaders;
using RockEngine.Engine.ECS;
using RockEngine.Rendering.Layers;
using RockEngine.Editor.GameObjects;
using RockEngine.Utils;
using RockEngine.DI;
using RockEngine.Physics;

namespace RockEngine.Editor.Layers
{
    public sealed class DefaultEditorLayer : ALayer, IDisposable
    {
        private readonly EngineWindow _window;
        public CameraTexture Screen;
        public PickingTexture PickingTexture;

        public override Layer Layer => Layer.Editor;

        public GameObject DebugCamera;
        private readonly PhysicsManager _physicsManager;
        private readonly AShaderProgram PickingShader;
        private readonly AShaderProgram SelectingShader;
        private readonly ImGuiRenderer _imguiLayer;

        public override int Order => 1;

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
            _physicsManager = IoC.Get<PhysicsManager>();
            _physicsManager.SetDebugRender(camera);
            _imguiLayer = new ImGuiRenderer(Application.GetMainWindow(),this, _physicsManager);
           
        }

        public override void OnRender(Scene scene)
        {
            DebugCamera.Update();
            DebugCamera.Render();
            PickingObjectPass(scene);

            MainRenderPass(scene);

            GettingObjectFromPicked(scene);
            _imguiLayer.OnRender();
        }

        private void GettingObjectFromPicked(IEnumerable<GameObject> gameObjects)
        {
            if(Input.IsButtonPressed(MouseButton.Left) &&
                ImGuiRenderer.IsMouseOnEditorScreen)
            {
                PickingTexture.PixelInfo pi = PickingTexture.ReadPixel((int)ImGuiRenderer.EditorScreenMousePos.X, (int)ImGuiRenderer.EditorScreenMousePos.Y);

                if((uint)pi.PrimID != 0 )
                {
                    var objID = (uint)pi.ObjectID;
                    var selected = gameObjects.FirstOrDefault(s => s.GameObjectID == objID);
                    _imguiLayer.SelectedGameObject = selected;
                }
                Console.WriteLine($"CLICKED ON: Primitive: {(uint)pi.PrimID}, ObjectID: {(uint)pi.ObjectID}, DrawIndex{(uint)pi.DrawID}");
            }
        }

        private void MainRenderPass(Scene scene)
        {
            Screen.BeginRenderToScreen();

            var selected = _imguiLayer.SelectedGameObject;
            if(selected != null)
            {
                GL.Clear(ClearBufferMask.StencilBufferBit);
                selected.IsActive = false;
            }
            scene.EditorLayerRender();

            OutlineSelectedGameObject(selected);
          
            if(EditorSettings.DrawCollisions)
            {
                _physicsManager.DebugRenderer.DebugRender();
            }

            Screen.EndRenderToScreen();
        }

        private void OutlineSelectedGameObject(GameObject? selected)
        {
            if(selected == null)
            {
                return;
            }
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.StencilTest);

            GL.StencilFunc(StencilFunction.Always, 1, 0xFF);
            GL.StencilMask(0xFF);
            selected.IsActive = true;
            selected.Render();
            GL.StencilFunc(StencilFunction.Notequal, 1, 0xFF);
            GL.StencilMask(0x00);
            GL.Disable(EnableCap.DepthTest);
            SelectingShader.Bind();
            selected.Render();
            SelectingShader.Unbind();
            GL.StencilMask(0xFF);
            GL.StencilFunc(StencilFunction.Always, 1, 0xFF);
            GL.Enable(EnableCap.DepthTest);
        }

        private void PickingObjectPass(IEnumerable<GameObject> gameObjects)
        {
            PickingTexture.CheckSize(Screen.ScreenTexture.Size);

            PickingShader.Bind();
            PickingTexture.BeginWrite();
            GL.Viewport(0, 0, Screen.ScreenTexture.Size.X, Screen.ScreenTexture.Size.Y);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
            for(int i = 0; i < gameObjects?.Count(); i++)
            {
                GameObject? gameObject = gameObjects.ElementAt(i);
                PickingData pd = new PickingData()
                {
                    gObjectIndex = gameObject.GameObjectID,
                    gDrawIndex = 0,
                };
                if(gameObject.GetComponent<Camera>() is not null)
                {
                    continue;
                }
                pd.SendData();
                gameObject.Render();

            }

            PickingTexture.EndWrite();
            PickingShader.Unbind();
        }

        public void Dispose()
        {
            Screen.Dispose();
        }
    }
}
