using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.Common.Utils;
using RockEngine.Rendering.OpenGL.Settings;

namespace RockEngine.Rendering.OpenGL.Buffers
{
    public sealed class VBO : ASetuppableGLObject<BufferSettings>
    {
        private nint syncObj;

        public override bool IsSetupped => Handle != IGLObject.EMPTY_HANDLE;

        public VBO(BufferSettings settings) : base(settings) { }

        public override VBO Bind(IRenderingContext context)
        {
            context.BindBuffer(BufferTarget.ArrayBuffer, Handle);
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
                GL.GetObjectLabel(ObjectLabelIdentifier.Buffer, Handle, 128, out int length, out string name);
                if(length == 0)
                {
                    name = $"IBO: ({Handle})";
                }
                Logger.AddLog($"Disposing {name}");
                if(syncObj != nint.Zero)
                {
                    Logger.AddLog($"Disposing fence sync");
                    GL.DeleteSync(syncObj);
                }
                GL.DeleteBuffers(1, ref _handle);
                Handle = IGLObject.EMPTY_HANDLE;
            });
            
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
            context.MapBufferRange(this, 0, Settings.BufferSize, flags, out buffer);
            return this;
        }

        public VBO UnmapBuffer(IRenderingContext context,ref nint buffer)
        {
            if(Handle != IGLObject.EMPTY_HANDLE && buffer != nint.Zero)
            {
                WaitBuffer(context);

                context.UnmapBuffer(Handle);
                buffer = nint.Zero;
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

        public unsafe VBO SendData(IRenderingContext context, Matrix4[ ] data, nint buffer)
        {
            throw new NotImplementedException("NOT IMPLEMENTED: TODO");
            fixed(Matrix4* matrixPtr = &data[0])
            {
                Matrix4* destinationPtr = (Matrix4*)buffer;
                System.Buffer.MemoryCopy(matrixPtr, destinationPtr, data.Length * sizeof(Matrix4), data.Length * sizeof(Matrix4));
            }

            /* unsafe
             {
                 Matrix4* matrixPtr = (Matrix4*)buffer;

                 for (int i = 0; i < data.Length; i++)
                 {
                     matrixPtr[i] = data[i];
                 }

             }*/

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
                context.ClientWaitSync(syncObj, ClientWaitSyncFlags.SyncFlushCommandsBit, 1, out waitReturn);
            }
            return this;
        }
        public VBO LockBuffer(IRenderingContext context)
        {
            context.DeleteSync(syncObj)
                .CreateFenceSync(SyncCondition.SyncGpuCommandsComplete, 0, out syncObj);
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
    }
}
