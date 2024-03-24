using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.Common.Utils;
using RockEngine.Rendering.OpenGL.Settings;

namespace RockEngine.Rendering.OpenGL.Buffers
{
    public sealed class IBO : ASetuppableGLObject<BufferSettings>
    {
        public override bool IsSetupped => Handle != IGLObject.EMPTY_HANDLE;

        public IBO(BufferSettings settings)
                   : base(settings)
        {
        }

        public override IBO Bind(IRenderingContext context)
        {
            context.BindBuffer(BufferTarget.DrawIndirectBuffer, Handle);
            return this;
        }

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
                if(!IsSetupped)
                {
                    return;
                }
                context.GetObjectLabel(ObjectLabelIdentifier.Buffer, Handle, 64, out int length, out string name);
                if(name.Length == 0)
                {
                    name = $"IBO: ({Handle})";
                }
                Logger.AddLog($"Disposing {name}");
                context.DeleteBuffer(_handle);
                _handle = IGLObject.EMPTY_HANDLE;
            });
        }

        ~IBO()
        {
            Dispose();
        }

        public override IBO SetLabel(IRenderingContext context)
        {
            string label = $"IBO: ({Handle})";
            Logger.AddLog($"Setupped {label}");

            context.ObjectLabel(ObjectLabelIdentifier.Buffer, Handle, label.Length, label);
            return this;
        }

        public override IBO Setup(IRenderingContext context)
        {
            context.CreateBuffer(out int handle);
            Handle = handle;
            return this;
        }

        public override IBO Unbind(IRenderingContext context)
        {
            context.BindBuffer(BufferTarget.DrawIndirectBuffer, IGLObject.EMPTY_HANDLE);
            return this;
        }
        public unsafe IBO SendData<T>(IRenderingContext context, T[] data) where T : struct
        {
            context.NamedBufferData(Handle, Settings.BufferSize, data, Settings.BufferUsageHint);
            return this;
        }

        public unsafe IBO SendData<T>(IRenderingContext context, T[ ] data, int size) where T : struct
        {
            context.NamedBufferData(Handle, size, data, Settings.BufferUsageHint);
            return this;
        }

        public unsafe IBO SendData(IRenderingContext context, Matrix4[ ] data, nint buffer)
        {
            throw new NotImplementedException("NOT IMPLEMENTED YET");
            //GL.NamedBufferData(Handle, Settings.BufferSize, data, BufferUsageHint.DynamicDraw);

            unsafe
            {
                Matrix4* matrixPtr = (Matrix4*)buffer;
                for(int i = 0; i < data.Length; i++)
                {
                    matrixPtr[i] = data[i];
                }
            }
            return this;
        }

        public override bool IsBinded(IRenderingContext context)
        {
            context.GetInteger(GetPName.DrawIndirectBufferBinding, out int handle);
            return handle == Handle;
        }
    }
}
