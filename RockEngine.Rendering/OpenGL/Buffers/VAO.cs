using OpenTK.Graphics.OpenGL4;

using RockEngine.Common.Utils;

namespace RockEngine.Rendering.OpenGL.Buffers
{
    public sealed class VAO : ASetuppableGLObject
    {
        public const int INSTANCE_MODELS_ATTRIBUTE = 10;

        public override bool IsSetupped => Handle != IGLObject.EMPTY_HANDLE;

        public override VAO Bind(IRenderingContext context)
        {
            context.Bind(this);
            return this;
        }

      
        public override VAO SetLabel(IRenderingContext context)
        {
            string label = $"VAO: ({Handle})";
            Logger.AddLog($"Setupped {label}");
            context.ObjectLabel(ObjectLabelIdentifier.VertexArray, Handle, label.Length, label);
            return this;
        }

        /// <summary>
        /// Creating in DSA style vertex array
        /// </summary>
        /// <returns>fluid syntax</returns>
        public override VAO Setup(IRenderingContext context)
        {
            context.CreateVertexArray(out int handle);
            Handle = handle;
            return this;
        }

        public override VAO Unbind(IRenderingContext context)
        {
            context.BindVAO(IGLObject.EMPTY_HANDLE);
            return this;
        }

        public VAO EnableVertexArrayAttrib(IRenderingContext context, int attribute)
        {
            context.EnableVertexArrayAttrib(Handle, attribute);
            return this;
        }

        public VAO VertexAttribDivisor(IRenderingContext context, int binding, int divisor)
        {
            context.VertexArrayAttribDivisor(Handle, binding, divisor);
            return this;
        }

        public VAO VertexAttribPointer(IRenderingContext context, int index, int size, VertexAttribPointerType type, bool normalized, int stride, nint offset)
        {
            context.VertexAttribPointer(index, size, type, normalized, stride, offset);
            return this;
        }

        public VAO VertexAttribPointer(IRenderingContext context, int index, int size, VertexAttribPointerType type, bool normalized, int stride, int offset)
        {
            context.VertexAttribPointer(index, size, type, normalized, stride, offset);
            return this;
        }

        public override bool IsBinded(IRenderingContext context)
        {
            context.GetInteger(GetPName.VertexArrayBinding, out int value);
            return value == Handle;
        }

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
            GL.GetObjectLabel(ObjectLabelIdentifier.VertexArray, Handle, 64, out int length, out string name);
            if(name.Length == 0)
            {
                name = $"VAO: ({Handle})";
            }
            Logger.AddLog($"Disposing {name}");
            GL.DeleteVertexArrays(1, ref _handle);
            _handle = IGLObject.EMPTY_HANDLE;
        }
        ~VAO()
        {
            Dispose();
        }
    }
}
