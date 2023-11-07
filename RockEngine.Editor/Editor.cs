﻿using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using RockEngine.DI;
using RockEngine.Engine.ECS;
using RockEngine.Engine.EngineStates;
using RockEngine.Engine;
using RockEngine.OpenGL.Shaders;
using RockEngine.Rendering.Layers;
using RockEngine.Physics;
using OpenTK.Graphics.OpenGL4;
using RockEngine.Editor.Layers;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RockEngine.Inputs;
using RockEngine.Utils;
using RockEngine.Editor.ImguiEditor;
using System.ComponentModel;

namespace RockEngine.Editor
{
    internal sealed class Editor : Application, IDisposable
    {
        private readonly EngineWindow _projectSelectingWindow;
        private VFShaderProgram DefaultShader;
        private PhysicsManager Physics;
        private readonly ProjectSelectorGUI _projectSelectingGUI;

        public Editor(string name, int width = 1280, int height = 720)
            : base(name, width,height)
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
                _projectSelectingWindow.Dispose();
            }
            else
            {
                _projectSelectingGUI.OnRender();
            }
        }

        public override void Start()
        {
            _projectSelectingWindow.Run();
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
            Physics = IoC.Get<PhysicsManager>();

            DefaultShader = new VFShaderProgram("TestShaderVert", new VertexShader("Resources/Shaders/TestVertex.vert"), new FragmentShader("Resources/Shaders/TestFragment.frag"));
   /*         AssetManager.CreateProject("Lor9nEngine", "C:\\Users\\Администратор\\Desktop\\LEProject", Guid.Parse("057F0D60-91EC-4DFF-A6BD-16A5C10970C1"));
            Scene scene = AssetManager.CreateScene("Test scene", "C:\\Users\\Администратор\\Desktop\\LEProject\\Assets\\Scenes", Guid.Parse("36CC1F73-C5E7-4D83-8448-855306097C1C"));

            Scene.ChangeScene(scene);
            var material = AssetManager.CreateMaterialAsset(PathInfo.PROJECT_ASSETS_PATH, "TestMeshMaterial");
            var testMesh = AssetManager.CreateMesh(ref Vertex3D.CubeVertices);

            var floor = new GameObject("Floor", new Transform(new Vector3(0, -50, 0), new Vector3(0), new Vector3(100, 1, 100)), new MeshComponent(testMesh), new MaterialComponent(AssetManager.CreateMaterialAsset(PathInfo.PROJECT_ASSETS_PATH, "MaterialFLoor")));
            floor.AddComponent(
                Physics.LocalCreateRigidBody(0,
                    floor.Transform.GetModelMatrix(),
                    new BoxShape(100)));
            scene.AddGameObject(floor);

            for(int i = 0; i < 5; i++)
            {
                var testTransform = new Transform(new Vector3(i, i, i));
                var testGameObject = new GameObject(
                    "TestGameObject",
                    testTransform,
                    new MeshComponent(testMesh),
                    new MaterialComponent(material));

                testGameObject.AddComponent(
                    Physics.LocalCreateRigidBody(1,
                    testTransform.GetModelMatrix().ClearScale(),
                    new BoxShape(testTransform.Scale))
                    );
                scene.AddGameObject(testGameObject);
            }
            scene.AddGameObject(new GameObject("MainCamera", new Camera(MainWindow.Size.X / (float)MainWindow.Size.Y)));

            Scene.MainCamera!.GetComponent<Camera>()!.LookAt(new Vector3(25), new Vector3(0), Vector3.UnitY);
            Scene.MainCamera.GetComponent<Camera>()!.UpdateVectors();

            scene.AddGameObject(
                    new GameObject(
                        "Direct light",
                        new DirectLight(new Vector3(1), 100_000),
                        new Transform(new Vector3(0, 50, 0), new Vector3(-1))));
            AssetManager.SaveAssetToFile(scene);*/

            EngineStateManager.RegisterStates(new DevepolerEngineState(), new PlayEngineState());
            Layers.AddLayer(new DefaultGameLayer());
            Layers.AddLayer(new DefaultEditorLayer());
        }

        protected override void Update(FrameEventArgs args)
        {
            if(EditorSettings.DrawCollisions)
            {
                foreach(var item in Scene.CurrentScene.GetGameObjects())
                {
                    var rb = item.GetComponent<EngineRigidBody>();
                    if(rb is not null)
                    {
                        Physics.World.DebugDrawObject(rb.WorldTransform, rb.CollisionShape, Vector3.UnitX);
                    }
                }
            }
            Physics.Update(Time.DeltaTime);
            EngineStateManager.UpdateState();
        }

        protected override void Render(FrameEventArgs args)
        {
            DefaultShader.Bind();
            Layers.OnRender();
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
