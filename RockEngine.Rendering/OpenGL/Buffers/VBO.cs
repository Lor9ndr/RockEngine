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

        public override VBO Bind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, Handle);
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
        }
        ~VBO()
        {
            Dispose();
        }

        public override VBO SetLabel()
        {
            string label = $"VBO: ({Handle})";
            Logger.AddLog($"Setupped {label}");
            GL.ObjectLabel(ObjectLabelIdentifier.Buffer, Handle, label.Length, label);
            return this;
        }

        public override VBO Setup()
        {
            GL.CreateBuffers(1, out int handle);
            Handle = handle;
            return this;
        }

        public VBO MapBuffer(BufferAccessMask flags, out nint buffer)
        {
            buffer = GL.MapNamedBufferRange(Handle, 0, Settings.BufferSize, flags);
            return this;
        }

        public VBO UnmapBuffer(ref nint buffer)
        {
            if(Handle != IGLObject.EMPTY_HANDLE && buffer != nint.Zero)
            {
                WaitBuffer();

                GL.UnmapNamedBuffer(Handle);
                buffer = nint.Zero;
                GL.DeleteSync(syncObj);
                syncObj = nint.Zero;
            }
            return this;
        }
        public VBO SetupBufferStorage<T>(BufferStorageFlags flags, T[ ] data) where T : struct
        {
            GL.NamedBufferStorage(Handle, Settings.BufferSize, data, flags);
            return this;
        }
        public VBO SetupBufferStorage(BufferStorageFlags flags, nint data)
        {
            GL.NamedBufferStorage(Handle, Settings.BufferSize, data, flags);
            return this;
        }

        public unsafe VBO SendData<T>(T[ ] data) where T : struct
        {
            GL.NamedBufferData(Handle, Settings.BufferSize, data, Settings.BufferUsageHint);
            return this;
        }

        public unsafe VBO SendData(Matrix4[ ] data, nint buffer)
        {
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

        public VBO WaitBuffer()
        {
            if(syncObj == nint.Zero)
            {
                Logger.AddError("To wait buffer, create a syncObj");
                return this;
            }
            WaitSyncStatus waitReturn = WaitSyncStatus.WaitFailed;
            while(waitReturn != WaitSyncStatus.AlreadySignaled && waitReturn != WaitSyncStatus.ConditionSatisfied)
            {
                waitReturn = GL.ClientWaitSync(syncObj, ClientWaitSyncFlags.SyncFlushCommandsBit, 1);
            }
            return this;
        }
        public VBO LockBuffer()
        {
            GL.DeleteSync(syncObj);
            syncObj = GL.FenceSync(SyncCondition.SyncGpuCommandsComplete, 0);
            return this;
        }

        public VBO SendData<T>(in T[ ] data) where T : struct
        {
            GL.NamedBufferData(Handle, Settings.BufferSize, data, Settings.BufferUsageHint);
            return this;
        }

        public override VBO Unbind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, IGLObject.EMPTY_HANDLE);
            return this;
        }

        public override bool IsBinded()
            => GL.GetInteger(GetPName.ArrayBufferBinding) == Handle;
    }
}
