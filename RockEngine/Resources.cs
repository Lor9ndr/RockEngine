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
            shader ??= new ShaderProgram(path.Name, path.SubShaders);
            return shader;
        }
    }
}
