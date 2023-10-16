namespace RockEngine.OpenGL.Shaders
{
    internal sealed class VFGShaderProgram : AShaderProgram
    {
        public VFGShaderProgram(string name, VertexShader vertexShader, FragmentShader fragmentShader, GeometryShader geometryShader) 
            :base(name, vertexShader, fragmentShader, geometryShader)
        {

        }
    }
}
