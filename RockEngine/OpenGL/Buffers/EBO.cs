using OpenTK.Graphics.OpenGL4;

using RockEngine.OpenGL.Settings;
using RockEngine.Utils;

namespace RockEngine.OpenGL.Buffers
{
    internal sealed class EBO : ASetuppableGLObject<BufferSettings>
    {

        public override bool IsSetupped => Handle != IGLObject.EMPTY_HANDLE;

        public EBO(BufferSettings settings) : base(settings) { }

        public override EBO Bind()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, Handle);
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
            GL.GetObjectLabel(ObjectLabelIdentifier.Buffer, Handle, 64, out int length, out string name);
            if (name.Length == 0)
            {
                name = $"IBO: ({Handle})";
            }
            Logger.AddLog($"Disposing {name}");
            GL.DeleteBuffers(1, ref _handle);
            // now Handle is 0 
            _handle = IGLObject.EMPTY_HANDLE;

            _disposed = true;
        }


        public override EBO SetLabel()
        {
            string label = $"EBO: ({Handle})";
            Logger.AddLog($"Setupped {label}");

            GL.ObjectLabel(ObjectLabelIdentifier.Buffer, Handle, label.Length, label);
            return this;
        }

        public override EBO Setup()
        {
            GL.CreateBuffers(1, out int handle);
            Handle = handle;
            return this;
        }

        public EBO SendData<T>(T[] data) where T : struct
        {
            GL.NamedBufferData(Handle, Settings.BufferSize, data, Settings.BufferUsageHint);
            return this;
        }
        public EBO SendData<T>(in T[] data) where T : struct
        {
            GL.NamedBufferData(Handle, Settings.BufferSize, data, Settings.BufferUsageHint);
            return this;
        }

        public override EBO Unbind()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IGLObject.EMPTY_HANDLE);
            return this;
        }
    }
}
