using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.OpenGL.Settings;
using RockEngine.Utils;

namespace RockEngine.OpenGL.Buffers
{
    internal sealed class IBO : ASetuppableGLObject<BufferSettings>
    {
        public override bool IsSetupped => Handle != IGLObject.EMPTY_HANDLE;

        public IBO(BufferSettings settings)
                   : base(settings)
        {
        }

        public override IBO Bind()
        {
            GL.BindBuffer(BufferTarget.DrawIndirectBuffer, Handle);
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
            if (!IsSetupped)
            {
                return;
            }
            GL.GetObjectLabel(ObjectLabelIdentifier.Buffer, Handle, 64, out int length, out string name);
            if (name.Length == 0)
            {
                name = $"IBO: ({Handle})";
            }
            Logger.AddLog($"Disposing {name}");
            GL.DeleteBuffers(1, ref _handle);
            _handle = IGLObject.EMPTY_HANDLE;

        }

        ~IBO()
        {
            Dispose();
        }

        public override IBO SetLabel()
        {
            string label = $"IBO: ({Handle})";
            Logger.AddLog($"Setupped {label}");

            GL.ObjectLabel(ObjectLabelIdentifier.Buffer, Handle, label.Length, label);
            return this;
        }

        public override IBO Setup()
        {
            GL.CreateBuffers(1, out int handle);
            Handle = handle;
            return this;
        }

        public override IBO Unbind()
        {
            GL.BindBuffer(BufferTarget.DrawIndirectBuffer, IGLObject.EMPTY_HANDLE);
            return this;
        }
        public unsafe IBO SendData<T>(T[] data) where T : struct
        {
            GL.NamedBufferData(Handle, Settings.BufferSize, data, Settings.BufferUsageHint);
            return this;
        }

        public unsafe IBO SendData<T>(T[] data, int size) where T : struct
        {
            GL.NamedBufferData(Handle, size, data, Settings.BufferUsageHint);
            return this;
        }

        public unsafe IBO SendData(Matrix4[] data, nint buffer)
        {
            //GL.NamedBufferData(Handle, Settings.BufferSize, data, BufferUsageHint.DynamicDraw);

            unsafe
            {
                Matrix4* matrixPtr = (Matrix4*)buffer;
                for (int i = 0; i < data.Length; i++)
                {
                    matrixPtr[i] = data[i];
                }
            }
            return this;
        }
    }
}
