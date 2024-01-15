using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

using RockEngine.Common;
using RockEngine.Common.Utils;
using RockEngine.Common.Vertices;
using RockEngine.DI;
using RockEngine.ECS;
using RockEngine.ECS.Assets;
using RockEngine.ECS.GameObjects;
using RockEngine.ECS.Layers;
using RockEngine.Editor.ImguiEditor;
using RockEngine.Editor.Layers;
using RockEngine.Engine.EngineStates;
using RockEngine.Inputs;
using RockEngine.Physics;
using RockEngine.Physics.Colliders;
using RockEngine.Rendering.OpenGL.Shaders;

using System.ComponentModel;
using System.Diagnostics;

namespace RockEngine.Editor
{
    internal sealed class Editor : Application, IDisposable
    {
        private readonly EngineWindow _projectSelectingWindow;
        private AShaderProgram DefaultShader;
        private PhysicsManager Physics;
        private readonly ProjectSelectorGUI _projectSelectingGUI;

        public Editor(string name, int width = 1280, int height = 720)
            : base(name, width, height)
        {
            MainWindow.IsVisible = false;
            MainWindow.KeyDown += MainWindow_KeyDown;

            _projectSelectingWindow = WindowManager.CreateWindow("Select a project", 200, 200);
            _projectSelectingWindow.Closing += SelectProjectClose;
            _projectSelectingWindow.RenderFrame += SelectProjectRenderFrame;
            _projectSelectingGUI = new ProjectSelectorGUI(_projectSelectingWindow);
            _projectSelectingWindow.CenterWindow();
            _projectSelectingWindow.WindowBorder = WindowBorder.Hidden;
        }

        private void SelectProjectClose(CancelEventArgs args)
        {
            _projectSelectingGUI.Dispose();
        }

        private void SelectProjectRenderFrame(FrameEventArgs args)
        {
            if(Project.CurrentProject is not null)
            {
                _projectSelectingWindow.Close();
            }
            else
            {
                _projectSelectingGUI.OnRender();
            }
        }

        public override void Start()
        {
            //_projectSelectingWindow.Run();
            //_projectSelectingWindow.Close();
            //_projectSelectingWindow.Dispose();
            MainWindow.CenterWindow();
            MainWindow.IsVisible = true;
            base.Start();
        }

        private void MainWindow_KeyDown(KeyboardKeyEventArgs obj)
        {
            if(obj.Key == Keys.M)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            }
            else if(obj.Key == Keys.N)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            }
            if(Input.IsKeyDown(Keys.LeftControl) && Input.IsKeyDown(Keys.Z))
            {
                EngineStateManager.Undo();
            }
            if(Input.IsKeyDown(Keys.LeftControl) && Input.IsKeyDown(Keys.Y))
            {
                EngineStateManager.Redo();
            }
        }

        protected override void Load()
        {
            MainWindow.VSync = VSyncMode.On;
            Physics = IoC.Get<PhysicsManager>();

            DefaultShader = ShaderProgram.GetOrCreate("TestShaderVert", new VertexShader("Resources/Shaders/TestVertex.vert"), new FragmentShader("Resources/Shaders/TestFragment.frag"));

            // Mock to load default project
            Scene scene = AssetManager.CreateScene("Test scene",
              "..\\LEProject\\Assets\\Scenes",
              Guid.Parse("36CC1F73-C5E7-4D83-8448-855306097C1C"));
            var project = AssetManager.CreateProject("Lor9nEngine",
                 "..\\LEProject",
                 Guid.Parse("057F0D60-91EC-4DFF-A6BD-16A5C10970C1"), scene);

            Material material;
            var testMesh = AssetManager.CreateMesh(ref Vertex3D.CubeVertices);

            var floor = new GameObject("Floor", new Transform(new Vector3(0, -50, 0), new Vector3(0), new Vector3(100, 1, 100)), new MeshComponent(testMesh), new MaterialComponent(AssetManager.CreateMaterialAsset(DefaultShader, PathsInfo.PROJECT_ASSETS_PATH, "MaterialFLoor")));
            floor.AddComponent(
                Physics.LocalCreateRigidBody(0,
                    floor.Transform.Position,
                    new BoxCollider(Vertex3D.GetMinPosition(testMesh.Vertices) * floor.Transform.Scale, Vertex3D.GetMaxPosition(testMesh.Vertices) * floor.Transform.Scale)));
            scene.Add(floor);

            Random rd = new Random();

            for(int i = 0; i > -1; i--)
            {
                for(int j = 0; j > -1; j--)
                {
                    for(int k = 0; k > -1; k--)
                    {
                        material = AssetManager.CreateMaterialAsset(DefaultShader, PathsInfo.PROJECT_ASSETS_PATH, "TestMeshMaterial");
                        material.ShaderData["material.albedo"] = new Vector3(-i+10, -j+5, -k+7);
                        var testTransform = new Transform(new Vector3(i, j, k));
                        var testGameObject = new GameObject(
                            "TestGameObject",
                            testTransform,
                            new MeshComponent(testMesh),
                            new MaterialComponent(material));

                        var body = testGameObject.AddComponent(
                            Physics.LocalCreateRigidBody(1f,
                            testTransform.Position,
                            new BoxCollider(Vertex3D.GetMinPosition(testMesh.Vertices) * testTransform.Scale, Vertex3D.GetMaxPosition(testMesh.Vertices) * testTransform.Scale))
                            );
                        body.AngularVelocity += new Vector3(100,100,100);
                        scene.Add(testGameObject);
                    }
                }
            }

            scene.Add(new GameObject("MainCamera", new Camera(MainWindow.Size.X / (float)MainWindow.Size.Y)));

            Scene.MainCamera!.GetComponent<Camera>()!.LookAt(new Vector3(25), new Vector3(0), Vector3.UnitY);
            Scene.MainCamera.GetComponent<Camera>()!.UpdateVectors();

            scene.Add(
                    new GameObject(
                        "Direct light",
                        new DirectLight(new Vector3(1), 100_000),
                        new Transform(new Vector3(0, 50, 0), new Vector3(-1))));
            project.FirstScene = scene;
            AssetManager.SaveAssetToFile(scene);
            AssetManager.SaveAssetToFile(project);
            Scene.ChangeScene(scene);
            Scene.ChangeScene(Project.CurrentProject.FirstScene);

            EngineStateManager.RegisterStates(new DevepolerEngineState(), new PlayEngineState());
            Layers.AddLayer(new DefaultGameLayer());
            Layers.AddLayer(new DefaultEditorLayer());
        }

        protected override void Update(FrameEventArgs args)
        {
            if(EditorSettings.DrawCollisions)
            {
                /* foreach(var item in Scene.CurrentScene.GetGameObjects())
                 {
                     var rb = item.GetComponent<EngineRigidBody>();
                     if(rb is not null)
                     {
                         Physics.World.DebugDrawObject(rb.WorldTransform, rb.CollisionShape, Vector3.UnitX);
                     }
                 }*/
            }
            Physics.Update((float)args.Time);
            EngineStateManager.UpdateState();
        }

        protected override void Render(FrameEventArgs args)
        {
            GL.ClearColor(EditorSettings.BackGroundColor);
            DefaultShader.Bind();
            Layers.OnRender(Scene.CurrentScene);
            DefaultShader.Unbind();
        }

        public void Dispose()
        {
            Scene.CurrentScene?.Dispose();
            foreach(var shader in AShaderProgram.AllShaders)
            {
                shader.Value.Dispose();
            }
            GC.SuppressFinalize(this);
            Physics.Dispose();
        }
    }
}
