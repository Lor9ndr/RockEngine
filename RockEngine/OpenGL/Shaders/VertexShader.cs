using OpenTK.Graphics.OpenGL4;

namespace RockEngine.OpenGL.Shaders
{
    internal sealed class VertexShader : BaseShaderType
    {
        public override ShaderType Type => base.Type;
        public VertexShader(string path)
            : base(path)
        {
        }
    }
}
