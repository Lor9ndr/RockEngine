using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using RockEngine.DI;
using RockEngine.Engine;
using RockEngine.Engine.EngineStates;
using RockEngine.OpenGL.Shaders;
using RockEngine.Physics;
using RockEngine.Rendering.Layers;

namespace RockEngine
{
    public sealed class Game : Application, IDisposable
    {
        private PhysicsManager Physics;

        public Game(string name, int width = 1280, int height = 720)
            : base(name, width, height)
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
        }

        protected override void Update(FrameEventArgs args)
        {
            Physics.Update(Time.DeltaTime);
            EngineStateManager.UpdateState();
        }

        protected override void Render(FrameEventArgs args)
        {
            Layers.OnRender();
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
