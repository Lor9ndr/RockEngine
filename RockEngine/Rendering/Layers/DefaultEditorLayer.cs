using OpenTK.Graphics.OpenGL4;
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

namespace RockEngine.Rendering.Layers
{
    internal sealed class DefaultEditorLayer : ALayer, IDisposable
    {
        public CameraTexture Screen;
        public PickingTexture PickingTexture;

        public GameObject DebugCamera;

        private readonly VFShaderProgram PickingShader;
        private readonly VFShaderProgram SelectingShader;
        public override int Order => 9;

        public DefaultEditorLayer()
        {
            Screen = new CameraTexture();
            PickingTexture = new PickingTexture(Game.MainWindow.Size);

            var basePicking = Path.Combine(PathConstants.RESOURCES, PathConstants.SHADERS, PathConstants.DEBUG, PathConstants.PICKING);
            var baseDebug = Path.Combine(PathConstants.RESOURCES, PathConstants.SHADERS, PathConstants.DEBUG, PathConstants.SELECTED_OBJECT);

            PickingShader = new VFShaderProgram("PickingShader",
                new VertexShader(Path.Combine(basePicking, "Picking.vert")),
                new FragmentShader(Path.Combine(basePicking, "Picking.frag")));

            SelectingShader = new VFShaderProgram("SelectingObjectShader",
                new VertexShader(Path.Combine(baseDebug, "Selected.vert")),
                new FragmentShader(Path.Combine(baseDebug, "Selected.frag")));

            var camera = new DebugCamera(Game.MainWindow.Size.X / (float)Game.MainWindow.Size.Y, Game.MainWindow);
            DebugCamera = new GameObject("DebugCamera", camera, new Transform(new Vector3(0, 10, 0)));
            camera.LookAt(new Vector3(15), new Vector3(0), Vector3.UnitY);
        }

        public override void OnRender()
        { 
            DebugCamera.UpdateOnDevelpmentState();
            DebugCamera.RenderOnEditorLayer();
            var gameObjects = Scene.CurrentScene?.GetGameObjects();
            PickingObjectPass(gameObjects);

            MainRenderPass();

            GettingObjectFromPicked(gameObjects);
        }

        private void GettingObjectFromPicked(List<GameObject> gameObjects)
        {
            if (Input.IsButtonPressed(MouseButton.Left) &&
                ImGuiLayer.IsMouseOnEditorScreen)
            {
                PickingTexture.PixelInfo pi = PickingTexture.ReadPixel((int)ImGuiLayer.EditorScreenMousePos.X, (int)ImGuiLayer.EditorScreenMousePos.Y);

                if (pi.PrimID != 0 && pi.ObjectID < gameObjects.Count)
                {
                    var selected = gameObjects.FirstOrDefault(s => s.GameObjectID == pi.ObjectID);
                    Game.LayerStack.GetLayer<ImGuiLayer>().SelectedGameObject = selected;

                }
            }
        }

        private void MainRenderPass()
        {
            Screen.BeginRenderToScreen();
            var selected = Game.LayerStack.GetLayer<ImGuiLayer>()!.SelectedGameObject;
            if (selected != null)
            {
                GL.Clear(ClearBufferMask.StencilBufferBit);
                selected.IsActive = false;
            }
            Scene.CurrentScene!.EditorLayerRender();

            OutlineSelectedGameObject(selected);

            Screen.EndRenderToScreen();
        }

        private void OutlineSelectedGameObject(GameObject? selected)
        {
            if (selected == null)
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

        private void PickingObjectPass(List<GameObject> gameObjects)
        {
            PickingTexture.CheckSize(Screen.ScreenTexture.Size);

            PickingShader.Bind();
            PickingTexture.BeginWrite();
            GL.Viewport(0, 0, Screen.ScreenTexture.Size.X, Screen.ScreenTexture.Size.Y);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
            for (int i = 0; i < gameObjects?.Count; i++)
            {
                GameObject? gameObject = gameObjects[i];
                PickingData pd = new PickingData()
                {
                    gObjectIndex = gameObject.GameObjectID - 1,
                    gDrawIndex = 1,
                };
                pd.SendData();
                gameObject.RenderOnEditorLayer();

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
