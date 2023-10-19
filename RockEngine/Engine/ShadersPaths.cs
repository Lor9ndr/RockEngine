using RockEngine.OpenGL.Shaders;

namespace RockEngine.Engine
{
    public record ShaderPath
    {
        public string Name { get;}
        public readonly BaseShaderType[] SubShaders;
        public ShaderPath(string name, params BaseShaderType[] shaders)
        {
            Name = name;
            SubShaders = shaders;
        }
    }

    public static class ShadersPaths
    {
        public static readonly ShaderPath Lit2DShader = new ShaderPath("Lit2DShader", new VertexShader("Resources\\Shaders\\Screen\\Screen.vert"), new FragmentShader("Resources\\Shaders\\Screen\\Screen.frag"));
    }
}
