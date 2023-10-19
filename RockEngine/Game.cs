using BulletSharp;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

using RockEngine.Assets;
using RockEngine.DI;
using RockEngine.Engine;
using RockEngine.Engine.ECS.GameObjects;
using RockEngine.Engine.EngineStates;
using RockEngine.OpenGL.Shaders;
using RockEngine.OpenGL.Vertices;
using RockEngine.Physics;
using RockEngine.Rendering.Layers;
using RockEngine.Utils;

using System.Buffers;

using RockEngine.Engine.ECS;

namespace RockEngine
{
    internal sealed class Game : IDisposable
    {
        public static EngineWindow MainWindow;

        public Matrix4[] models;

        readonly Random rd = new Random();
        public static LayerStack LayerStack;
        readonly ArrayPool<Matrix4> pool = ArrayPool<Matrix4>.Shared;

        private VFShaderProgram DefaultShader;
        private PhysicsManager Physics;

        public Game(string name, int width = 1280, int height = 720)
        {
            MainWindow = WindowManager.CreateWindow(name, width, height);
            MainWindow.RenderFrame += Render;
            MainWindow.UpdateFrame += Update;
            MainWindow.Load += Load;
            MainWindow.KeyDown += MainWindow_KeyDown;
        }

        private void MainWindow_KeyDown(KeyboardKeyEventArgs obj)
        {
            if (obj.Key == OpenTK.Windowing.GraphicsLibraryFramework.Keys.M)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            }
            else if (obj.Key == OpenTK.Windowing.GraphicsLibraryFramework.Keys.N)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            }
        }

        public void Start()
        {
            MainWindow.Run();
        }

        private void Load()
        {
            Physics = IoC.Get<PhysicsManager>();
            Project p = new Project("Lor9nEngine", "C:\\Users\\Администратор\\Desktop\\LEProject", Guid.Parse("057F0D60-91EC-4DFF-A6BD-16A5C10970C1"));
            AssetManager.SaveAssetToFile(p);
            var scene = new Scene("Test scene", "C:\\Users\\Администратор\\Desktop\\LEProject\\Assets\\Scenes", Guid.Parse("36CC1F73-C5E7-4D83-8448-855306097C1C"));

            Scene.ChangeScene(scene);

            EngineStateManager.RegisterStates(new DevepolerEngineState(), new PlayEngineState());
            LayerStack = new LayerStack();
            LayerStack.AddLayer(new DefaultEditorLayer());
            LayerStack.AddLayer(new DefaultGameLayer());
            LayerStack.AddLayer(new ImGuiLayer());

            DefaultShader = new VFShaderProgram("TestShaderVert", new VertexShader("Resources/Shaders/TestVertex.vert"), new FragmentShader("Resources/Shaders/TestFragment.frag"));

            var vertices = Vertex3D.CubeVertices;

            var material = AssetManager.CreateMaterialAsset(DefaultShader, PathInfo.PROJECT_ASSETS_PATH, "TestMeshMaterial");
            var testMesh = AssetManager.CreateMesh(ref vertices);
            for (int i = -25; i < 25; i++)
            {
                var testTransform = new Transform(new Vector3(i, i, i));
                var testGameObject = new GameObject("TestGameObject", testTransform, testMesh, material);
                testGameObject.AddComponent(Physics.LocalCreateRigidBody(1, testTransform.GetModelMatrix(), new BoxShape(testTransform.Scale)));
                scene.AddGameObject(testGameObject);
            }
            scene.AddGameObject(new GameObject("MainCamera", new Camera(MainWindow.Size.X / (float)MainWindow.Size.Y)));

            var floor = new GameObject("Floor", testMesh, AssetManager.CreateMaterialAsset(DefaultShader, PathInfo.PROJECT_ASSETS_PATH, "MaterialFLoor"));
            floor.Transform.Scale = new Vector3(100, 1, 100);
            floor.AddComponent(Physics.LocalCreateRigidBody(0, floor.Transform.GetModelMatrix(), new BoxShape(floor.Transform.Scale)));
            scene.AddGameObject(floor);
            Scene.MainCamera!.GetComponent<Camera>()!.LookAt(new Vector3(25), new Vector3(0), Vector3.UnitY);
            Scene.MainCamera.GetComponent<Camera>()!.UpdateVectors();
            
            scene.AddGameObject(new GameObject("Direct light", new DirectLight() { Intensity = 100_000, LightColor = new Vector3(1) }, new Transform(new Vector3(0,50,0), new Vector3(-1))));
            AssetManager.SaveAssetToFile(scene);
        }

        private void Update(FrameEventArgs args)
        {
            /* Parallel.For(0, models.Length, i =>
             {
                 var t = new Transform
                 {
                     Position = models[i].ExtractTranslation(),
                     RotationQuaternion = models[i].ExtractRotation()
                 };
                 float radius = 10.0f; // Adjust the radius of the sphere as needed
                 float angle = (float)i / models.Length * 2 * MathF.PI * Time.DeltaTime; // Calculate the angle based on the index
                 float x = radius * MathF.Cos(angle); // Calculate the x-coordinate

                 // Update the position of the transform
                 t.Rotate(Vector3.UnitX, MathHelper.RadiansToDegrees(45 * 0.001f * Time.DeltaTime));
                 models[i] = t.GetModelMatrix();
             });
             Scene.CurrentScene.GetGameObjects()[1].GetComponent<MeshComponent>().SetInstanceMatrices(in models);
             pool.Return(models,false);*/

            Physics.Update(Time.DeltaTime);
            EngineStateManager.UpdateState();
        }

        private void Render(FrameEventArgs args)
        {
            DefaultShader.Bind();
            LayerStack.OnRender();
            DefaultShader.Unbind();
        }

        public void Dispose()
        {
            Scene.CurrentScene?.Dispose();
            foreach (var shader in AShaderProgram.AllShaders)
            {
                shader.Value.Dispose();
            }
            GC.SuppressFinalize(this);
            Physics.ExitPhysics();
        }
    }
}
