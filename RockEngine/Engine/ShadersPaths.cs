namespace RockEngine.Engine
{
    public record ShaderPath(string Name,string Vertex, string Fragment, string? Geometry = null);

    public static class ShadersPaths
    {
        public static ShaderPath Lit2DShader = new ShaderPath("Lit2DShader", "Resources\\Shaders\\Screen\\Screen.vert", "Resources\\Shaders\\Screen\\Screen.frag");
    }
}
