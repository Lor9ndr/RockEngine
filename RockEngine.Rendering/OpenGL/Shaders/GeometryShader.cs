using OpenTK.Graphics.OpenGL4;

namespace RockEngine.Rendering.OpenGL.Shaders
{
    internal sealed class GeometryShader : BaseShaderType
    {
        public override ShaderType Type => ShaderType.GeometryShader;
        public GeometryShader(string path) : base(path)
        {
        }
    }
}
