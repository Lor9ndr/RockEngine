using OpenTK.Graphics.OpenGL4;

using RockEngine.OpenGL.Settings;
using RockEngine.OpenGL.Shaders;
using RockEngine.Utils;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace RockEngine.OpenGL.Buffers
{
    internal sealed class UBO<T> : ASetuppableGLObject<BufferSettings> where T : struct
    {
        public override bool IsSetupped => Handle != 0;

        public UBO(BufferSettings settings)
            : base(settings) { }
        public override UBO<T> Setup()
        {
            GL.CreateBuffers(1, out int handle);
            Handle = handle;
            GL.NamedBufferData(Handle, Settings.BufferSize, nint.Zero, Settings.BufferUsageHint);

            GL.BindBufferRange(BufferRangeTarget.UniformBuffer, Settings.BindingPoint, Handle, nint.Zero, Settings.BufferSize);
            foreach (var shader in AShaderProgram.AllShaders)
            {
                shader.Value.Bind();
                var index = GL.GetUniformBlockIndex(shader.Value.Handle, Settings.BufferName);
                if (index != -1)
                {
                    GL.UniformBlockBinding(shader.Value.Handle, index, Settings.BindingPoint);
                }
            }

            return this;
        }

        public UBO<T> SendData(T data)
        {
            var ptr = Marshal.AllocHGlobal(Settings.BufferSize);
            Marshal.StructureToPtr(data, ptr, true);

            GL.NamedBufferData(Handle, Settings.BufferSize, ptr, Settings.BufferUsageHint);
            Marshal.FreeHGlobal(ptr);
            return this;
        }
        public UBO<T> SendData<Tother>(ref Tother data, int size) where Tother : struct
        {
            // Switch size to other data size
            GL.BindBufferRange(BufferRangeTarget.UniformBuffer, Settings.BindingPoint, Handle, nint.Zero, size);
            GL.NamedBufferSubData(Handle, nint.Zero, size, ref data);
            // Return to the base size 
            GL.BindBufferRange(BufferRangeTarget.UniformBuffer, Settings.BindingPoint, Handle, nint.Zero, Settings.BufferSize);
            return this;

        }
        public UBO<T> SendData(T[] data, int size)
        {
            GL.NamedBufferSubData(Handle, nint.Zero, size, data);
            return this;
        }
        public UBO<T> SendData(ref nint data, int size)
        {
            GL.NamedBufferSubData(Handle, nint.Zero, size, data);
            return this;
        }

        public UBO<T> SendData<TSub>([NotNull, DisallowNull] TSub subdata, nint offset, int size)
        {
            var ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(subdata, ptr, true);

            GL.NamedBufferSubData(Handle, offset, size, ptr);
            Marshal.FreeHGlobal(ptr);
            return this;
        }

        public override UBO<T> SetLabel()
        {
            var label = $"UBO ({Handle}), Name: {Settings.BufferName}";
            Logger.AddLog($"Setupped {label}");

            GL.ObjectLabel(ObjectLabelIdentifier.Buffer, Handle, label.Length, label);
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
            GL.GetObjectLabel(ObjectLabelIdentifier.Buffer, Handle, 128, out int length, out string name);
            if (length == 0)
            {
                name = $"UBO: ({Handle})";
            }
            Logger.AddLog($"Disposing {name}");
            GL.DeleteBuffers(1, ref _handle);
            _handle = IGLObject.EMPTY_HANDLE;
        }
        ~UBO()
        {
            Dispose();
        }

        public override UBO<T> Bind()
        {
            GL.BindBuffer(BufferTarget.UniformBuffer, Handle);
            return this;
        }

        public override IGLObject Unbind()
        {
            GL.BindBuffer(BufferTarget.UniformBuffer, IGLObject.EMPTY_HANDLE);
            return this;
        }
    }
}
