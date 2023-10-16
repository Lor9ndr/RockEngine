namespace RockEngine.OpenGL.Shaders
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    internal sealed class ShaderUniformAttribute : Attribute
    {
        public string UniformName { get; set; }
        public ShaderUniformAttribute(string uniformName)
        {
            UniformName = uniformName;
        }
    }
}
