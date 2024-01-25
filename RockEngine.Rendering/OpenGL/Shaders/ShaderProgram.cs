using OpenTK.Graphics.OpenGL4;

namespace RockEngine.Rendering.OpenGL.Shaders
{
    /// <summary>
    /// Класс шейдера
    /// </summary>
    public sealed class ShaderProgram : AShaderProgram
    {

        #region Ctor

        private ShaderProgram(string name, params BaseShaderType[ ] baseShaders)
            : base(name, baseShaders)
        {
        }

        #endregion

        public static ShaderProgram GetOrCreate(string name, params BaseShaderType[ ] baseShaders)
        {
            if(AllShaders.TryGetValue(name, out var shader))
            {
                return (ShaderProgram)shader;
            }
            return new ShaderProgram(name, baseShaders);
        }
        public override bool IsBinded()
            => GL.GetInteger(GetPName.CurrentProgram) == Handle;
    }
}