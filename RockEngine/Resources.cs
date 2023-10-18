using RockEngine.Engine;
using RockEngine.OpenGL.Shaders;

namespace RockEngine
{
    internal static class Resources
    {
        public static AShaderProgram? GetShader(string name)
        {
            if(AShaderProgram.AllShaders.TryGetValue(name, out var shader))
            {
               return shader; 
            }
            return null;
        }

        public static AShaderProgram GetOrCreateShader(ShaderPath path)
        {
            var shader = GetShader(path.Name);
            if(shader is null)
            {
                if(!string.IsNullOrEmpty(path.Geometry))
                {
                    shader = new ShaderProgram(path.Name, new VertexShader(path.Vertex), new FragmentShader(path.Fragment), new GeometryShader(path.Geometry));
                }
                else
                {
                    shader = new ShaderProgram(path.Name, new VertexShader(path.Vertex), new FragmentShader(path.Fragment));
                }
            }
            return shader;
        }
    }
}
