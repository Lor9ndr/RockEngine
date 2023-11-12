namespace RockEngine.OpenGL.Shaders
{
    /// <summary>
    /// Класс шейдера
    /// </summary>
    public sealed class ShaderProgram : AShaderProgram
    {

        #region Ctor

        private ShaderProgram(string name, params BaseShaderType[] baseShaders)
            : base(name, baseShaders)
        {
        }

        #endregion

        public static AShaderProgram GetOrCreate(string name, params BaseShaderType[ ] baseShaders)
        {
            if(AllShaders.TryGetValue(name, out var shader))
            {
                return shader;
            }
            return new ShaderProgram(name, baseShaders);
        }
    }
}