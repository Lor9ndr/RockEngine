using OpenTK.Mathematics;

using RockEngine.ECS.GameObjects;

using System.Diagnostics;

namespace RockEngine.ECS.Layers
{
    public sealed class DefaultGameLayer : ALayer, IDisposable
    {
        public override int Order => 0;

        public CameraTexture Screen;

        public Stopwatch Watcher;

        public DefaultGameLayer()
        {
            Screen = new CameraTexture(new Vector2i(1280, 720));
            Watcher = new Stopwatch();
        }

        public override void OnRender(Scene scene)
        {
            Watcher.Restart();
            Screen.BeginRenderToScreen();
            scene.Render();
            Screen.EndRenderToScreen();
            Watcher.Stop();
        }

        public void Dispose()
        {
            Screen.Dispose();
        }
    }
}
