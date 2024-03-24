using OpenTK.Graphics.OpenGL4;

namespace RockEngine.Rendering.OpenGL.Shaders
{
    public sealed class FragmentShader : BaseShaderType
    {
        public override ShaderType Type => ShaderType.FragmentShader;
        public FragmentShader(string path) : base(path)
        {
        }
    }
}
