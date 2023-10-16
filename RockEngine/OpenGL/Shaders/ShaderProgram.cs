namespace RockEngine.OpenGL.Shaders
{
    /// <summary>
    /// Класс шейдера
    /// </summary>
    public sealed class ShaderProgram : AShaderProgram, IDisposable
    {

        #region Ctor

        public ShaderProgram(string name, params BaseShaderType[] baseShaders)
            : base(name, baseShaders)
        {
        }

        #endregion
    }
}