using RockEngine.Rendering.OpenGL.Settings;

namespace RockEngine.Rendering.OpenGL.Textures
{
    public abstract class BaseTexture : ASetuppableGLObject<TextureSettings>, IGLObject
    {
        public BaseTexture(TextureSettings settings):base(settings)
        {
        }
    }
}
