using OpenTK.Mathematics;

using RockEngine.ECS;
using RockEngine.ECS.GameObjects;

namespace RockEngine.ECS.Layers
{
    public sealed class DefaultGameLayer : ALayer, IDisposable
    {
        public override int Order => 0;

        public CameraTexture Screen;

        public DefaultGameLayer()
        {
            Screen = new CameraTexture(new Vector2i(1280, 720));
        }

        public override void OnRender(Scene scene)
        {
            Screen.BeginRenderToScreen();
            scene.Render();
            Screen.EndRenderToScreen();
        }

        public void Dispose()
        {
            Screen.Dispose();
        }
    }
}
