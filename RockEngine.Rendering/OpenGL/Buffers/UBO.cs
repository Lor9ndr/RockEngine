using OpenTK.Graphics.OpenGL4;

using RockEngine.Common.Utils;
using RockEngine.Rendering.OpenGL.Settings;
using RockEngine.Rendering.OpenGL.Shaders;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace RockEngine.Rendering.OpenGL.Buffers
{
    public sealed class UBO<T> : ASetuppableGLObject<BufferSettings> where T : struct
    {
        public override bool IsSetupped => Handle != 0;

        public UBO(BufferSettings settings)
            : base(settings) { }

        public override UBO<T> Setup(IRenderingContext context)
        {
            context.CreateBuffer(out int handle);
            Handle = handle;
            if(Handle == IGLObject.EMPTY_HANDLE)
            {
                throw new Exception($"Unable to create Uniform buffer of type <{typeof(T)}>");
            }

            foreach(var shader in AShaderProgram.AllShaders)
            {
                context.GetUniformBlockIndex(shader.Value.Handle, Settings.BufferName, out int index)
                    .GetProgram(shader.Value.Handle, GetProgramParameterName.ActiveUniformBlocks, out int maxCount);

                if(index != -1 && index < maxCount)
                {
                    context.UniformBlockBinding(shader.Value.Handle, index, Settings.BindingPoint)
                        .GetActiveUniformBlock(shader.Value.Handle, index, ActiveUniformBlockParameter.UniformBlockDataSize, out int size);
                    Settings.BufferSize = (int)MathF.Max(size, Settings.BufferSize);

                    shader.Value.BoundUniformBuffers[Handle] = this;
                }
            }
            context.NamedBufferData(Handle, Settings.BufferSize, nint.Zero, Settings.BufferUsageHint)
                .BindBufferRange(BufferRangeTarget.UniformBuffer, Settings.BindingPoint, Handle, nint.Zero, Settings.BufferSize);
            return this;
        }

        public UBO<T> SendData(IRenderingContext context, T data)
        {
            if(!BindToActiveShaderIfNot(context))
            {
                return this;
            }

            var ptr = Marshal.AllocHGlobal(Settings.BufferSize);
            Marshal.StructureToPtr(data, ptr, true);
            context.NamedBufferData(Handle, Settings.BufferSize, ptr, Settings.BufferUsageHint);
            Marshal.FreeHGlobal(ptr);
            return this;
        }

        public UBO<T> SendData(IRenderingContext context, object[ ] data)
        {
            throw new NotImplementedException("NOT IMPLEMENTED: TODO");
            if(!BindToActiveShaderIfNot(context))
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

        public UBO<T> SendData<TOther>(IRenderingContext context, ref TOther data, int size) where TOther : struct
        {
            if(!BindToActiveShaderIfNot(context))
            {
                return this;
            }

            // Switch size to other data size
            context.BindBufferRange(BufferRangeTarget.UniformBuffer, Settings.BindingPoint, Handle, nint.Zero, size)
                .NamedBufferSubData(Handle, nint.Zero, size, ref data)
                .BindBufferRange(BufferRangeTarget.UniformBuffer, Settings.BindingPoint, Handle, nint.Zero, Settings.BufferSize);
            return this;
        }

        public bool BindToActiveShaderIfNot(IRenderingContext context)
        {
            var shader = AShaderProgram.ActiveShader;
            return shader is not null && BindBufferToShader(context, shader);
        }

        public bool BindBufferToShader(IRenderingContext context, AShaderProgram shader)
        {
            if(!shader.BoundUniformBuffers.ContainsKey(Handle))
            {
                context.GetUniformBlockIndex(shader.Handle, Settings.BufferName, out int index);
                if(index != -1)
                {
                    context.UniformBlockBinding(shader.Handle, index, Settings.BindingPoint);

                    // Add this line to add the uniform buffer to the cache
                    shader.BoundUniformBuffers[Handle] = this;
                    return true;
                }
                return false;
            }
            return true;
        }

        public UBO<T> SendData(IRenderingContext context, T[ ] data, int size)
        {
            if(!BindToActiveShaderIfNot(context))
            {
                return this;
            }

            context.NamedBufferSubData(Handle, nint.Zero, size, data);
            return this;
        }
        public UBO<T> SendData(IRenderingContext context, ref nint data, int size)
        {
            if(!BindToActiveShaderIfNot(context))
            {
                return this;
            }
            context.NamedBufferSubData(Handle, nint.Zero, size, data);
            return this;
        }

        public UBO<T> SendData<TSub>(IRenderingContext context, [NotNull, DisallowNull] TSub subdata, nint offset, int size)
        {
            if(!BindToActiveShaderIfNot(context))
            {
                return this;
            }
            var ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(subdata, ptr, true);
            context.NamedBufferSubData(Handle, offset, size, ptr);
            Marshal.FreeHGlobal(ptr);
            return this;
        }

        public override UBO<T> SetLabel(IRenderingContext context)
        {
            var label = $"UBO ({Handle}), Name: {Settings.BufferName}";
            Logger.AddLog($"Setupped {label}");

            context.ObjectLabel(ObjectLabelIdentifier.Buffer, Handle, label.Length, label);
            return this;
        }

        public override void Dispose(bool disposing, IRenderingContext? context = null)
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
            context.GetObjectLabel(ObjectLabelIdentifier.Buffer, Handle, 128, out int length, out string name);
            if(length == 0)
            {
                name = $"UBO: ({Handle})";
            }
            Logger.AddLog($"Disposing {name}");
            context.DeleteBuffer(_handle);
            _handle = IGLObject.EMPTY_HANDLE;
        }
        ~UBO()
        {
            Dispose();
        }

        public override UBO<T> Bind(IRenderingContext context)
        {
            throw new Exception($"Unable to bind uniform buffer, it is sends data directly at {nameof(SendData)} method");
        }

        public override IGLObject Unbind(IRenderingContext context)
        {
            throw new Exception($"Unable to UnBind uniform buffer, it is sends data directly at {nameof(SendData)} method");
        }

        public override bool IsBinded(IRenderingContext context)
           => false;
    }
}
