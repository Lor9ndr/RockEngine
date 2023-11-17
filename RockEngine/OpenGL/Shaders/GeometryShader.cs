﻿using OpenTK.Graphics.OpenGL4;

namespace RockEngine.OpenGL.Shaders
{
    internal sealed class GeometryShader : BaseShaderType
    {
        public override ShaderType Type => ShaderType.GeometryShader;
        public GeometryShader(string path) : base(path)
        {
        }
        public override bool IsBinded()
            => GL.GetInteger(GetPName.CurrentProgram) == _mainProgramHandle;
    }
}
