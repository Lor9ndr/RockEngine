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

        public BaseShaderType Setup(int mainProgramHandle)
        {
            _mainProgramHandle = mainProgramHandle;

            var shaderSource = GetShaderText();

            Handle = GL.CreateShader(Type);
            GL.ShaderSource(Handle, shaderSource);
            CompileShader(Handle, _path);
            GL.AttachShader(_mainProgramHandle, Handle);
            return this;
        }

        public string GetShaderText()
        {
            return File.ReadAllText(_path);
        }

        /// <summary>
        /// Компиляция шейддера
        /// </summary>
        /// <param name="shader"> индекс шейдера</param>
        /// <exception cref="Exception">Если ошибка компиляции выдает ошибку</exception>
        protected void CompileShader(int shader, string path)
        {
            // Try to compile the shader
            GL.CompileShader(shader);

            // Check for compilation errors
            GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
            if(code != (int)All.True)
            {
                // We can use `GL.GetShaderInfoLog(shader)` to get information about the error.
                var infoLog = GL.GetShaderInfoLog(shader);
                Logger.AddError($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}, path: {path}");
            }
        }

        public string GetFilePath() => _path;

        protected override void Dispose(bool disposing)
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
            GL.GetObjectLabel(ObjectLabelIdentifier.Shader, Handle, 64, out int length, out string name);
            if(name.Length == 0)
            {
                name = $"Shader: ({Handle})";
            }
            Logger.AddLog($"Disposing {name}");
            GL.DetachShader(_mainProgramHandle, Handle);
            GL.DeleteShader(Handle);
            Handle = IGLObject.EMPTY_HANDLE;
            _disposed = true;
        }

        public override BaseShaderType SetLabel()
        {
            string name = $"Shader {Type}({Handle})";
            GL.ObjectLabel(ObjectLabelIdentifier.Shader, Handle, name.Length, name);
            return this;
        }

        public override BaseShaderType Bind()
        {
            throw new NotImplementedException();
        }

        public override IGLObject Unbind()
        {
            throw new NotImplementedException();
        }
    }
}
