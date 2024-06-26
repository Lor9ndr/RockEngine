﻿using OpenTK.Graphics.OpenGL4;

namespace RockEngine.Rendering.OpenGL.Shaders
{
    public class ComputeShader : BaseShaderType
    {
        public override ShaderType Type => ShaderType.ComputeShader;
        public ComputeShader(string path)
            : base(path)
        {
        }
    }
}
