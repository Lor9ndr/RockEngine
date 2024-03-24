using OpenTK.Mathematics;

using RockEngine.ECS.GameObjects;
using RockEngine.Rendering;

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

        public override Task OnRender(Scene scene)
        {
            IRenderingContext.Render(context =>
            {
                Watcher.Restart();
                Screen.BeginRenderToScreen(context);
                scene.Render(context);
                Screen.EndRenderToScreen(context);
                //Screen.DisplayScreen(context);
                Watcher.Stop();
            });
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Screen.Dispose();
        }
    }
}
