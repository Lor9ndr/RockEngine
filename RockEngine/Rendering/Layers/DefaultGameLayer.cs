using RockEngine.Engine;
using RockEngine.Engine.ECS.GameObjects;

namespace RockEngine.Rendering.Layers
{
    public sealed class DefaultGameLayer : ALayer, IDisposable
    {
        public override int Order => 0;

        public override Layer Layer => Layer.Default;

        public CameraTexture Screen;

        public DefaultGameLayer()
        {
            Screen = new CameraTexture(new OpenTK.Mathematics.Vector2i(1280, 720));
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
