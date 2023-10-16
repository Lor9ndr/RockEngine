namespace RockEngine.OpenGL.Shaders
{
    /// <summary>
    /// Vertex and fragment shader
    /// </summary>
    internal sealed class VFShaderProgram : AShaderProgram
    {

        public VFShaderProgram(string name, VertexShader vertexShader, FragmentShader fragmentShader)
         : base(name, vertexShader, fragmentShader)
        {
        }
    }
}
