using BulletSharp;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using RockEngine.Assets;
using RockEngine.DI;
using RockEngine.Engine.ECS.GameObjects;
using RockEngine.Engine.ECS;
using RockEngine.Engine.EngineStates;
using RockEngine.Engine;
using RockEngine.OpenGL.Shaders;
using RockEngine.OpenGL.Vertices;
using RockEngine.Rendering.Layers;
using RockEngine.Physics;
using OpenTK.Graphics.OpenGL4;
using RockEngine.Editor.Layers;

namespace RockEngine.Editor
{
    internal sealed class Editor : Application, IDisposable
    {
        private VFShaderProgram DefaultShader;
        private PhysicsManager Physics;

        public Editor(string name, int width = 1280, int height = 720)
            : base(name, width,height)
        {
            MainWindow.KeyDown += MainWindow_KeyDown;
        }

        private void MainWindow_KeyDown(KeyboardKeyEventArgs obj)
        {
            if(obj.Key == OpenTK.Windowing.GraphicsLibraryFramework.Keys.M)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            }
            else if(obj.Key == OpenTK.Windowing.GraphicsLibraryFramework.Keys.N)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            }
        }

        protected override void Load()
        {
            Physics = IoC.Get<PhysicsManager>();
            AssetManager.CreateProject("Lor9nEngine", "C:\\Users\\Администратор\\Desktop\\LEProject", Guid.Parse("057F0D60-91EC-4DFF-A6BD-16A5C10970C1"));
            Scene scene = AssetManager.CreateScene("Test scene", "C:\\Users\\Администратор\\Desktop\\LEProject\\Assets\\Scenes", Guid.Parse("36CC1F73-C5E7-4D83-8448-855306097C1C"));

            Scene.ChangeScene(scene);

            EngineStateManager.RegisterStates(new DevepolerEngineState(), new PlayEngineState());
            Layers.AddLayer(new DefaultGameLayer());
            Layers.AddLayer(new DefaultEditorLayer());

            DefaultShader = new VFShaderProgram("TestShaderVert", new VertexShader("Resources/Shaders/TestVertex.vert"), new FragmentShader("Resources/Shaders/TestFragment.frag"));

            var vertices = Vertex3D.CubeVertices;

            var material = AssetManager.CreateMaterialAsset(PathInfo.PROJECT_ASSETS_PATH, "TestMeshMaterial");
            var testMesh = AssetManager.CreateMesh(ref vertices);
            for(int i = -25; i < 25; i++)
            {
                var testTransform = new Transform(new Vector3(i, i, i));
                var testGameObject = new GameObject("TestGameObject", testTransform, new MeshComponent(testMesh), new MaterialComponent(material));
                testGameObject.AddComponent(Physics.LocalCreateRigidBody(100, testTransform.GetModelMatrix().ClearScale(), new BoxShape(testTransform.Scale)));
                scene.AddGameObject(testGameObject);
            }
            scene.AddGameObject(new GameObject("MainCamera", new Camera(MainWindow.Size.X / (float)MainWindow.Size.Y)));

            var floor = new GameObject("Floor", new MeshComponent(testMesh), new MaterialComponent(AssetManager.CreateMaterialAsset(PathInfo.PROJECT_ASSETS_PATH, "MaterialFLoor")));
            floor.Transform.Scale = new Vector3(100, 1, 100);
            floor.AddComponent(Physics.LocalCreateRigidBody(0, floor.Transform.GetModelMatrix(), new BoxShape(floor.Transform.Scale / 2)));
            scene.AddGameObject(floor);
            Scene.MainCamera!.GetComponent<Camera>()!.LookAt(new Vector3(25), new Vector3(0), Vector3.UnitY);
            Scene.MainCamera.GetComponent<Camera>()!.UpdateVectors();

            scene.AddGameObject(
                    new GameObject(
                        "Direct light",
                        new DirectLight(new Vector3(1), 100_000),
                        new Transform(new Vector3(0, 50, 0), new Vector3(-1))));
            AssetManager.SaveAssetToFile(scene);
        }

        protected override void Update(FrameEventArgs args)
        {
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
