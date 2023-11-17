using OpenTK.Graphics.OpenGL4;

namespace RockEngine.OpenGL.Shaders
{
    public sealed class VertexShader : BaseShaderType
    {
        public override ShaderType Type => base.Type;
        public VertexShader(string path)
            : base(path)
        {
        }
        public override bool IsBinded()
          => GL.GetInteger(GetPName.CurrentProgram) == _mainProgramHandle;
    }
}
