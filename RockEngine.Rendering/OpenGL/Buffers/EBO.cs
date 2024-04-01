using OpenTK.Graphics.OpenGL4;

using RockEngine.Common.Utils;
using RockEngine.Rendering.OpenGL.Settings;

namespace RockEngine.Rendering.OpenGL.Buffers
{
    public sealed class EBO : ASetuppableGLObject<BufferSettings>
    {
        public override bool IsSetupped => Handle != IGLObject.EMPTY_HANDLE;

        public EBO(BufferSettings settings) : base(settings) { }

        public override EBO Bind(IRenderingContext context)
        {
            context.BindBuffer(BufferTarget.ElementArrayBuffer, Handle);
            return this;
        }

        public override void Dispose(bool disposing, IRenderingContext context = null)
        {
            if(context is null)
            {
                IRenderingContext.Update(context =>
                {
                    InternalDispose(disposing, context);
                });
            }
            else
            {
                InternalDispose(disposing, context);
            }
        }

        private void InternalDispose(bool disposing, IRenderingContext context)
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

            context.GetObjectLabel(ObjectLabelIdentifier.Buffer, Handle, 64, out _, out string name);
            if(name.Length == 0)
            {
                name = $"IBO: ({Handle})";
            }
            Logger.AddLog($"Disposing {name}");
            context.DeleteBuffer(_handle);
            // now Handle is 0 
            _handle = IGLObject.EMPTY_HANDLE;

            _disposed = true;

        }

        public override EBO SetLabel(IRenderingContext context)
        {
            string label = $"EBO: ({Handle})";
            Logger.AddLog($"Setupped {label}");

            context.ObjectLabel(ObjectLabelIdentifier.Buffer, Handle, label.Length, label);
            return this;
        }

        public override EBO Setup(IRenderingContext context)
        {
            context.CreateBuffer(out int handle);
            Handle = handle;
            return this;
        }

        public EBO SendData<T>(IRenderingContext context, T[] data) where T : struct
        {
            context.NamedBufferData(Handle, Settings.BufferSize, data, Settings.BufferUsageHint);
            return this;
        }

        public override EBO Unbind(IRenderingContext context)
        {
            context.BindBuffer(BufferTarget.ElementArrayBuffer, IGLObject.EMPTY_HANDLE);
            return this;
        }

        public override bool IsBinded(IRenderingContext context)
        {
            context.GetInteger(GetPName.ElementArrayBufferBinding, out int value);
            return value == Handle;
        }
    }
}
