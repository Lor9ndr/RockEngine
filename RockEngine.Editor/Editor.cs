using Ninject.Activation;

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
using RockEngine.Rendering;
using RockEngine.Rendering.OpenGL.Shaders;

namespace RockEngine.Editor
{
    internal sealed class Editor : Application, IDisposable
    {
        private readonly EngineWindow _projectSelectingWindow;
        private AShaderProgram DefaultShader;
        private readonly PhysicsManager Physics;
        private readonly ProjectSelectorGUI _projectSelectingGUI;

        public Editor(string name, int width = 1920, int height = 1080)
            : base(name, width, height)
        {
            MainWindow.IsVisible = false;
            MainWindow.KeyDown += MainWindow_KeyDown;
            Physics = IoC.Get<PhysicsManager>();

            // _projectSelectingWindow = WindowManager.CreateWindow("Select a project", 200, 200);
            // _projectSelectingWindow.Closing += SelectProjectClose;
            // _projectSelectingWindow.RenderFrame += SelectProjectRenderFrame;
            // _projectSelectingGUI = new ProjectSelectorGUI(_projectSelectingWindow);
            // _projectSelectingWindow.CenterWindow();
            // _projectSelectingWindow.WindowBorder = WindowBorder.Hidden;
        }

      /*  private void SelectProjectClose(CancelEventArgs args)
        {
            _projectSelectingGUI.Dispose();
        }*/

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

        protected override async Task Initilized()
        {
            MainWindow.VSync = VSyncMode.Off;

            LoadShaders();

            // Mock to load default project
            Scene scene = await AssetManager.CreateSceneAsync("Test scene",
              "..\\LEProject\\Assets\\Scenes",
              Guid.Parse("36CC1F73-C5E7-4D83-8448-855306097C1C"))
                .ConfigureAwait(false);

            var project = await AssetManager.CreateProjectAsync("Lor9nEngine",
                 "..\\LEProject",
                 Guid.Parse("057F0D60-91EC-4DFF-A6BD-16A5C10970C1"), scene)
                .ConfigureAwait(false);

            Material material;
            var testMesh = await AssetManager.CreateMesh(Vertex3D.CubeVertices).ConfigureAwait(false);
            var floormaterial = await AssetManager.CreateMaterialAsset(DefaultShader, PathsInfo.PROJECT_ASSETS_PATH, "MaterialFLoor").ConfigureAwait(false);
            IRenderingContext.Update(ctx =>
            {
                // Ensure GameObject creation is on the main thread
                var floor = new GameObject("Floor", new Transform(new Vector3(0, -50, 0), new Vector3(0), new Vector3(100, 1, 100)), new MeshComponent(testMesh), new MaterialComponent(floormaterial));
                floor.AddComponent(
                    Physics.LocalCreateRigidBody(0,
                        floor.Transform.Position,
                        new OBB(floor.Transform.Position, floor.Transform.Scale)));
                scene.Add(floor);
            });

            await InitCubesBox(scene, testMesh).ConfigureAwait(false);

            IRenderingContext.Update(ctx =>
            {
                scene.Add(new GameObject("MainCamera", new Camera(MainWindow.Size.X / (float)MainWindow.Size.Y)));

                Scene.MainCamera!.GetComponent<Camera>()!.LookAt(new Vector3(25), new Vector3(0), Vector3.UnitY);
                Scene.MainCamera.GetComponent<Camera>()!.UpdateVectors();

                scene.Add(
                        new GameObject(
                            "Direct light",
                            new DirectLight(new Vector3(1), 100_000),
                            new Transform(new Vector3(0, 50, 0), new Vector3(-1))));
                project.FirstScene = scene;

                Scene.ChangeScene(scene);
                Scene.ChangeScene(Project.CurrentProject.FirstScene);
                EngineStateManager.RegisterStates(new DevepolerEngineState(), new PlayEngineState());
                Layers.AddLayer(new DefaultGameLayer());
                Layers.AddLayer(new DefaultEditorLayer());
            });
        }

      

        protected override void Update(FrameEventArgs args)
        {
            OpenGLDispatcher.ExecuteUpdateCommands();

            AssetManager.LoadAssetsToOpenGL();
            //Physics.Update((float)args.Time);
            Physics.World.ColliderRenderer.Update();
            EngineStateManager.UpdateState();

        }

        protected override async void Render(FrameEventArgs args)
        {
            IRenderingContext.Render(context =>
            {
                context.ClearColor(EditorSettings.BackGroundColor);
                context.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
                DefaultShader?.Bind(context);
            });

            await Layers.OnRender(Scene.CurrentScene).ConfigureAwait(false);
            IRenderingContext.Render(context =>
            {
                DefaultShader?.Unbind(context);
            });

            OpenGLDispatcher.ExecuteRenderCommands();
        }

        private void LoadShaders()
        {
            // Shaders init
            IRenderingContext.Update(ctx =>
            {
                DefaultShader = ShaderProgram.GetOrCreate("TestShaderVert",
                    new VertexShader("Resources/Shaders/TestVertex.vert"),
                    new FragmentShader("Resources/Shaders/TestFragment.frag"));

                DefaultShader.Setup(ctx);

                ShaderProgram.GetOrCreate("Lit2DShader",
                    new VertexShader("Resources\\Shaders\\Screen\\Screen.vert"),
                    new FragmentShader("Resources\\Shaders\\Screen\\Screen.frag"))
                .Setup(ctx);

                ShaderProgram.GetOrCreate("Lit2DShader",
                new VertexShader("Resources\\Shaders\\Screen\\Screen.vert"),
                new FragmentShader("Resources\\Shaders\\Screen\\Screen.frag"))
                .Setup(ctx);

                var basePicking = new PathInfo(PathConstants.RESOURCES / PathConstants.SHADERS / PathConstants.DEBUG / PathConstants.PICKING);
                var baseDebug = new PathInfo(PathConstants.RESOURCES / PathConstants.SHADERS / PathConstants.DEBUG / PathConstants.SELECTED_OBJECT);

                ShaderProgram.GetOrCreate("PickingShader",
              new VertexShader(Path.Combine(basePicking, "Picking.vert")),
                        new FragmentShader(Path.Combine(basePicking, "Picking.frag")))
                .Setup(ctx);

                ShaderProgram.GetOrCreate("SelectingObjectShader",
                    new VertexShader(Path.Combine(baseDebug, "Selected.vert")),
                    new FragmentShader(Path.Combine(baseDebug, "Selected.frag")))
                .Setup(ctx);

                ShaderProgram.GetOrCreate("DebugBox",
                new VertexShader(PathConstants.RESOURCES + PathConstants.SHADERS + "/BoxRenderShader/DebugBox.vert"),
                new FragmentShader(PathConstants.RESOURCES + PathConstants.SHADERS + "/BoxRenderShader/DebugBox.frag"))
                .Setup(ctx);

                baseDebug = new PathInfo(PathConstants.RESOURCES) / PathConstants.SHADERS / PathConstants.DEBUG / PathConstants.GIZMO;

                ShaderProgram.GetOrCreate("GizmoShader",
                   new VertexShader(baseDebug / "Gizmo.vert"),
                    new FragmentShader(baseDebug / "Gizmo.frag"))
               .Setup(ctx);
            });
            //Force load shaders
            OpenGLDispatcher.ExecuteUpdateCommands();
        }

        private async Task InitCubesBox(Scene scene, Mesh testMesh)
        {
            Material material;
            // Nested loops for GameObject creation
            for(int i = 0; i > -2; i--)
            {
                for(int j = 0; j > -2; j--)
                {
                    for(int k = 0; k > -2; k--)
                    {
                        material = await AssetManager.CreateMaterialAsset(DefaultShader, PathsInfo.PROJECT_ASSETS_PATH, "TestMeshMaterial(" +( i + j + k) + ")")
                            .ConfigureAwait(false);
                        IRenderingContext.Update(ctx =>
                        {
                            material.ShaderData["material.albedo"] = new Vector3(Random.Shared.NextSingle(), Random.Shared.NextSingle(), Random.Shared.NextSingle());
                            var testTransform = new Transform(new Vector3(Random.Shared.NextSingle() * 10, Random.Shared.NextSingle() * 10, Random.Shared.NextSingle() * 10));
                            var testGameObject = new GameObject(
                                "TestGameObject",
                                testTransform,
                                new MeshComponent(testMesh),
                                new MaterialComponent(material));

                            var body = testGameObject.AddComponent(
                                Physics.LocalCreateRigidBody(10f,
                                testTransform.Position,
                                new OBB(testTransform.Position, testTransform.Scale))
                                );
                            scene.Add(testGameObject);
                        });
                    }
                }
            }
        }

        public override void Dispose()
        {
            Scene.CurrentScene?.Dispose();
            foreach(var shader in AShaderProgram.AllShaders)
            {
                shader.Value.Dispose();
            }
            base.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
