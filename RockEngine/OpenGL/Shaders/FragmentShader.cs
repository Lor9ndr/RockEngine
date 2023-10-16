using OpenTK.Graphics.OpenGL4;

namespace RockEngine.OpenGL.Shaders
{
    internal sealed class FragmentShader : BaseShaderType
    {
        public override ShaderType Type => ShaderType.FragmentShader;
        public FragmentShader(string path) : base(path)
        {
        }
    }
}
