using OpenTK.Graphics.OpenGL4;

using RockEngine.Common.Utils;

namespace RockEngine.Rendering.OpenGL.Shaders
{
    public abstract class BaseShaderType : AGLObject
    {
        public virtual ShaderType Type => ShaderType.VertexShader;
        public bool IsSetupped => Handle != IGLObject.EMPTY_HANDLE;

        protected int _mainProgramHandle;
        private readonly string _path;

        public BaseShaderType(string path)
        {
            _path = path;
        }

        public BaseShaderType Setup(IRenderingContext context, int mainProgramHandle)
        {
            _mainProgramHandle = mainProgramHandle;
            var shaderSource = GetShaderText();

            context.CreateShader(Type, out int handle);
            Handle = handle;
            context.ShaderSource(Handle, shaderSource)
                .CompileShader(Handle, out int status);
            if(status != (int)All.True)
            {
                context.GetShaderInfoLog(Handle, out string infoLog);
                Logger.AddError($"Error occurred while compiling Shader({Handle}).\n\n{infoLog}, path: {_path}");

            }
            context.AttachShader(_mainProgramHandle, Handle);

            return this;
        }

        public string GetShaderText()
        {
            return File.ReadAllText(_path);
        }

        public string GetFilePath() => _path;

        protected override void Dispose(bool disposing)
        {
            IRenderingContext.Update(context =>
            {
                if(_disposed)
                {
                    return;
                }
                if(disposing)
                {
                    // Освободите управляемые ресурсы здесь
                }

                if(!IsSetupped)
                {
                    return;
                }

                context.GetObjectLabel(ObjectLabelIdentifier.Shader, Handle, 64, out int length, out string name);
                if(name.Length == 0)
                {
                    name = $"Shader: ({Handle})";
                }
                Logger.AddLog($"Disposing {name}");
                context.DetachShader(_mainProgramHandle, Handle);
                context.DeleteShader(Handle);
                Handle = IGLObject.EMPTY_HANDLE;
                _disposed = true;
            });
        }

        public override BaseShaderType SetLabel(IRenderingContext context)
        {
            string name = $"Shader {Type}({Handle})";
            context.ObjectLabel(ObjectLabelIdentifier.Shader, Handle, name.Length, name);
            return this;
        }
        public override bool IsBinded(IRenderingContext context)
        {
            context.GetInteger(GetPName.CurrentProgram, out int value);
            return value == _mainProgramHandle;
        }

        public override BaseShaderType Bind(IRenderingContext context)
        {
            throw new NotImplementedException();
        }

        public override IGLObject Unbind(IRenderingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
