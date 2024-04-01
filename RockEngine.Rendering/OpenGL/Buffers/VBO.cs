using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.Common.Utils;
using RockEngine.Rendering.OpenGL.Settings;

namespace RockEngine.Rendering.OpenGL.Buffers
{
    public sealed class VBO : ASetuppableGLObject<BufferSettings>
    {
        private nint syncObj;
        private nint _mappedBuffer = nint.Zero;

        public override bool IsSetupped => Handle != IGLObject.EMPTY_HANDLE;

        public bool IsMapped { get;  private set; }

        public VBO(BufferSettings settings) : base(settings) { }

        public override VBO Bind(IRenderingContext context)
        {
            context.BindBuffer(BufferTarget.ArrayBuffer, Handle);
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
            if(syncObj != nint.Zero)
            {
                UnmapBuffer(context);
            }
            context.GetObjectLabel(ObjectLabelIdentifier.Buffer, Handle, 128, out int length, out string name);
            if(length == 0)
            {
                name = $"IBO: ({Handle})";
            }
            Logger.AddLog($"Disposing {name}");
            if(syncObj != nint.Zero)
            {
                Logger.AddLog($"Disposing fence sync");
                context.DeleteSync(syncObj);
            }
            context.DeleteBuffer(Handle);
            Handle = IGLObject.EMPTY_HANDLE;
        }
       
        ~VBO()
        {
            Dispose();
        }

        public override VBO SetLabel(IRenderingContext context)
        {
            string label = $"VBO: ({Handle})";
            Logger.AddLog($"Setupped {label}");
            context.ObjectLabel(ObjectLabelIdentifier.Buffer, Handle, label.Length, label);
            return this;
        }

        public override VBO Setup(IRenderingContext context)
        {
            context.CreateBuffer(out int handle);
            Handle = handle;
            return this;
        }

        public VBO MapBuffer(IRenderingContext context, BufferAccessMask flags, out nint buffer)
        {
            if(IsMapped)
            {
                buffer = _mappedBuffer;
                return this;
            }
            context.MapBufferRange(this, 0, Settings.BufferSize, flags, out buffer);
            _mappedBuffer = buffer;
            IsMapped = true;
            return this;
        }

        public VBO UnmapBuffer(IRenderingContext context)
        {
            if(Handle != IGLObject.EMPTY_HANDLE && _mappedBuffer != nint.Zero)
            {
                if(syncObj != nint.Zero)
                {
                    WaitBuffer(context);
                }

                context.UnmapBuffer(Handle);
                IsMapped = false;
                _mappedBuffer = nint.Zero;
                context.DeleteSync(syncObj);
                syncObj = nint.Zero;
            }
            return this;
        }
        public VBO SetupBufferStorage<T>(IRenderingContext context,BufferStorageFlags flags, T[ ] data) where T : struct
        {
            context.NamedBufferStorage(Handle, Settings.BufferSize, data, flags);
            return this;
        }
        public VBO SetupBufferStorage(IRenderingContext context, BufferStorageFlags flags, nint data)
        {
            context.NamedBufferStorage(Handle, Settings.BufferSize, data, flags);
            return this;
        }

        public unsafe VBO SendData<T>(IRenderingContext context, T[] data) where T : struct
        {
            context.NamedBufferData(Handle, Settings.BufferSize, data, Settings.BufferUsageHint);
            return this;
        }

        public unsafe VBO SendDataMappedBuffer(IRenderingContext context, Matrix4[] data, int startIndex, int byteOffset, int size)
        {
            if(startIndex < 0 || startIndex >= data.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex), "startIndex is out of the bounds of the data array.");
            }

            int actualSize = Math.Min(size, (data.Length - startIndex) * 64);

            Matrix4* destinationPtr = (Matrix4*)(_mappedBuffer + byteOffset);

            fixed(Matrix4* sourcePtr = data)
            {
                Matrix4* adjustedSourcePtr = sourcePtr + startIndex;
                System.Buffer.MemoryCopy(adjustedSourcePtr, destinationPtr, actualSize, actualSize);
            }

            return this;
        }
        public unsafe VBO SendDataMappedBuffer<T>(IRenderingContext context, T[] data, int startIndex, int byteOffset, int size) where T :unmanaged
        {
            if(startIndex < 0 || startIndex >= data.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex), "startIndex is out of the bounds of the data array.");
            }

            int actualSize = Math.Min(size, (data.Length - startIndex) * sizeof(T));

            T* destinationPtr = (T*)(_mappedBuffer + byteOffset);

            fixed(T* sourcePtr = data)
            {
                T* adjustedSourcePtr = sourcePtr + startIndex;
                System.Buffer.MemoryCopy(adjustedSourcePtr, destinationPtr, actualSize, actualSize);
            }

            return this;
        }

        public VBO SendData(IRenderingContext context, Matrix4[] data, int offset, int size)
        {
            context.NamedBufferSubData(Handle, offset, size, data);
            return this;
        }

        public VBO WaitBuffer(IRenderingContext context)
        {
            if(syncObj == nint.Zero)
            {
                Logger.AddError("To wait buffer, create a syncObj");
                return this;
            }
            WaitSyncStatus waitReturn = WaitSyncStatus.WaitFailed;
            while(waitReturn != WaitSyncStatus.AlreadySignaled && waitReturn != WaitSyncStatus.ConditionSatisfied)
            {
                context.ClientWaitSync(syncObj, ClientWaitSyncFlags.SyncFlushCommandsBit, 1000, out waitReturn);
            }
            return this;
        }
        public VBO LockBuffer(IRenderingContext context)
        {
            if(syncObj != nint.Zero)
            {
                context.DeleteSync(syncObj);
            }
            context.CreateFenceSync(SyncCondition.SyncGpuCommandsComplete, 0, out syncObj);
            return this;
        }

        public VBO SendData<T>(IRenderingContext context, in T[ ] data) where T : struct
        {
            context.NamedBufferData(Handle, Settings.BufferSize, data, Settings.BufferUsageHint);
            return this;
        }

        public override VBO Unbind(IRenderingContext context)
        {
            context.BindBuffer(BufferTarget.ArrayBuffer, IGLObject.EMPTY_HANDLE);
            return this;
        }

        public override bool IsBinded(IRenderingContext context)
        {
            context.GetInteger(GetPName.ArrayBufferBinding, out int value);
            return value == Handle;
        }

        public nint GetMappedBuffer()
        {
            return _mappedBuffer;
        }

        public void Resize(IRenderingContext context, int requiredSize)
        {
            // Check if the current buffer size is already the required size
            if(Settings.BufferSize == requiredSize)
            {
                return; // No resizing needed
            }

            // Create a new buffer with the required size
            context.CreateBuffer(out int newBufferHandle);
            context.BindBuffer(BufferTarget.ArrayBuffer, newBufferHandle);
            context.NamedBufferData(newBufferHandle, requiredSize, IntPtr.Zero, Settings.BufferUsageHint);

            // Optionally, copy data from the old buffer to the new one
            if(IsMapped && _mappedBuffer != nint.Zero)
            {
                // Assuming you want to copy the entire content of the old buffer to the new one
                // Adjust the size parameter as needed based on the actual data size to copy
                int sizeToCopy = Math.Min(requiredSize, Settings.BufferSize);
                context.CopyNamedBufferSubData(Handle, newBufferHandle, 0, 0, sizeToCopy);
            }

            // Delete the old buffer
            Dispose(false,context);

            // Update the VBO instance to use the new buffer
            Handle = newBufferHandle;
            Settings.BufferSize = requiredSize; // Update the buffer size setting

            // If the buffer was mapped, you may need to remap it or handle this scenario appropriately
            _mappedBuffer = nint.Zero; // Reset the mapped buffer pointer
            IsMapped = false; // Reset the mapped state
        }
    }
}
