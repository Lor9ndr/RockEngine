using OpenTK.Graphics.OpenGL4;

using RockEngine.Utils;

namespace RockEngine.OpenGL.Buffers
{
    internal sealed class VAO : ASetuppableGLObject
    {
        public const int INSTANCE_MODELS_ATTRIBUTE = 10;

        public override bool IsSetupped => Handle != IGLObject.EMPTY_HANDLE;

        public override VAO Bind()
        {
            GL.BindVertexArray(Handle);
            return this;
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                // Освободите управляемые ресурсы здесь
            }

            if (!IsSetupped)
            {
                return;
            }
            GL.GetObjectLabel(ObjectLabelIdentifier.VertexArray, Handle, 64, out int length, out string name);
            if (name.Length == 0)
            {
                name = $"IBO: ({Handle})";
            }
            Logger.AddLog($"Disposing {name}");
            GL.DeleteVertexArrays(1, ref _handle);
            _handle = IGLObject.EMPTY_HANDLE;
        }
        ~VAO()
        {
            Dispose();
        }
        public override VAO SetLabel()
        {
            string label = $"VAO: ({Handle})";
            Logger.AddLog($"Setupped {label}");
            GL.ObjectLabel(ObjectLabelIdentifier.VertexArray, Handle, label.Length, label);
            return this;
        }

        /// <summary>
        /// Creating in DSA style vertex array
        /// </summary>
        /// <returns>fluid syntax</returns>
        public override VAO Setup()
        {
            GL.CreateVertexArrays(1, out int handle);
            Handle = handle;
            return this;
        }

        public override VAO Unbind()
        {
            GL.BindVertexArray(IGLObject.EMPTY_HANDLE);
            return this;
        }

        public VAO EnableVertexArrayAttrib(int attribute)
        {
            GL.EnableVertexArrayAttrib(Handle, attribute);
            return this;
        }

        public VAO VertexAttribDivisor(int binding, int divisor)
        {
            GL.VertexArrayBindingDivisor(Handle, binding, divisor);
            return this;
        }

        public VAO VertexAttribPointer(int index, int size, VertexAttribPointerType type, bool normalized, int stride, nint offset)
        {
            GL.VertexAttribPointer(index, size, type, normalized, stride, offset);
            return this;
        }

        public VAO VertexAttribPointer(int index, int size, VertexAttribPointerType type, bool normalized, int stride, int offset)
        {
            GL.VertexAttribPointer(index, size, type, normalized, stride, offset);
            return this;
        }
    }
}
