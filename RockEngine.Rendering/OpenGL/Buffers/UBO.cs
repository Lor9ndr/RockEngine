using OpenTK.Graphics.OpenGL4;

using RockEngine.Rendering.OpenGL.Settings;
using RockEngine.Rendering.OpenGL.Shaders;
using RockEngine.Common.Utils;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace RockEngine.Rendering.OpenGL.Buffers
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
            if(Handle == IGLObject.EMPTY_HANDLE)
            {
                throw new Exception($"Unable to create Uniform buffer of type <{typeof(T)}>");
            }

            foreach(var shader in AShaderProgram.AllShaders)
            {
                var index = GL.GetUniformBlockIndex(shader.Value.Handle, Settings.BufferName);
                GL.GetProgram(shader.Value.Handle, GetProgramParameterName.ActiveUniformBlocks, out int maxCount);

                if(index != -1 && index < maxCount)
                {
                    GL.UniformBlockBinding(shader.Value.Handle, index, Settings.BindingPoint);
                    GL.GetActiveUniformBlock(shader.Value.Handle, index, ActiveUniformBlockParameter.UniformBlockDataSize, out int size);
                    Settings.BufferSize = (int)MathF.Max(size, Settings.BufferSize);

                    // Add this line to add the uniform buffer to the cache
                    shader.Value.BoundUniformBuffers[Handle] = this;
                }
            }
            GL.NamedBufferData(Handle, Settings.BufferSize, nint.Zero, Settings.BufferUsageHint);

            GL.BindBufferRange(BufferRangeTarget.UniformBuffer, Settings.BindingPoint, Handle, nint.Zero, Settings.BufferSize);
            return this;
        }

        public UBO<T> SendData(T data)
        {
            if(!BindToActiveShaderIfNot())
            {
                return this;
            }

            var ptr = Marshal.AllocHGlobal(Settings.BufferSize);
            Marshal.StructureToPtr(data, ptr, true);

            GL.NamedBufferData(Handle, Settings.BufferSize, ptr, Settings.BufferUsageHint);
            Marshal.FreeHGlobal(ptr);
            return this;
        }

        public UBO<T> SendData(object[ ] data)
        {
            throw new NotImplementedException("NOT IMPLEMENTED: TODO");
            if(!BindToActiveShaderIfNot())
            {
                return this;
            }
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            for(int i = 0; i < data.Length; i++)
            {
                var ptr = Marshal.UnsafeAddrOfPinnedArrayElement(data, i);
                var arraySegment = new ArraySegment<object>(data);
                int offset = arraySegment.Offset + i;
                GL.NamedBufferSubData(Handle, offset, Settings.BufferSize, ptr);
            }
            handle.Free();

            //Marshal.StructureToPtr(data, ptr, false);

            return this;
        }

        public UBO<T> SendData<Tother>(ref Tother data, int size) where Tother : struct
        {
            if(!BindToActiveShaderIfNot())
            {
                return this;
            }

            // Switch size to other data size
            GL.BindBufferRange(BufferRangeTarget.UniformBuffer, Settings.BindingPoint, Handle, nint.Zero, size);
            GL.NamedBufferSubData(Handle, nint.Zero, size, ref data);
            // Return to the base size 
            GL.BindBufferRange(BufferRangeTarget.UniformBuffer, Settings.BindingPoint, Handle, nint.Zero, Settings.BufferSize);
            return this;
        }

        public bool BindToActiveShaderIfNot()
        {
            var shader = AShaderProgram.ActiveShader;
            return shader is not null && BindBufferToShader(shader);
        }

        public bool BindBufferToShader(AShaderProgram shader)
        {
            if(!shader.BoundUniformBuffers.ContainsKey(Handle))
            {
                var index = GL.GetUniformBlockIndex(shader.Handle, Settings.BufferName);
                if(index != -1)
                {
                    GL.UniformBlockBinding(shader.Handle, index, Settings.BindingPoint);

                    // Add this line to add the uniform buffer to the cache
                    shader.BoundUniformBuffers[Handle] = this;
                    return true;
                }
                return false;
            }
            return true;
        }

        public UBO<T> SendData(T[ ] data, int size)
        {
            if(!BindToActiveShaderIfNot())
            {
                return this;
            }

            GL.NamedBufferSubData(Handle, nint.Zero, size, data);
            return this;
        }
        public UBO<T> SendData(ref nint data, int size)
        {
            if(!BindToActiveShaderIfNot())
            {
                return this;
            }
            GL.NamedBufferSubData(Handle, nint.Zero, size, data);
            return this;
        }

        public UBO<T> SendData<TSub>([NotNull, DisallowNull] TSub subdata, nint offset, int size)
        {
            if(!BindToActiveShaderIfNot())
            {
                return this;
            }
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
            GL.GetObjectLabel(ObjectLabelIdentifier.Buffer, Handle, 128, out int length, out string name);
            if(length == 0)
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
            throw new Exception($"Unable to bind uniform buffer, it is sends data directly at {nameof(SendData)} method");
        }

        public override IGLObject Unbind()
        {
            throw new Exception($"Unable to UnBind uniform buffer, it is sends data directly at {nameof(SendData)} method");
        }

        public override bool IsBinded()
           => false;
    }
}
