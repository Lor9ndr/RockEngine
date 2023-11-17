using OpenTK.Graphics.OpenGL4;

namespace RockEngine.OpenGL.Shaders
{
    public sealed class FragmentShader : BaseShaderType
    {
        public override ShaderType Type => ShaderType.FragmentShader;
        public FragmentShader(string path) : base(path)
        {
        }
        public override bool IsBinded()
            => GL.GetInteger(GetPName.CurrentProgram) == _mainProgramHandle;
    }
}
