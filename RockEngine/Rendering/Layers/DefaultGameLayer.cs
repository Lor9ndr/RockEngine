using RockEngine.Engine;
using RockEngine.Engine.ECS.GameObjects;

namespace RockEngine.Rendering.Layers
{
    internal sealed class DefaultGameLayer : ALayer, IDisposable
    {
        public override int Order => 10;
        public CameraTexture Screen;

        public DefaultGameLayer()
        {
            Screen = new CameraTexture();
        }

        public override void OnRender()
        {
            Screen.BeginRenderToScreen();
            Scene.CurrentScene.Render();
            Screen.EndRenderToScreen();
        }

        public void Dispose()
        {
            Screen.Dispose();
        }
    }
}
