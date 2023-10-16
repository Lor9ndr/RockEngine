using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.OpenGL.Settings;
using RockEngine.Utils;

namespace RockEngine.OpenGL.Buffers
{
    internal sealed class RBO : ASetuppableGLObject<RenderBufferSettings>
    {
        public override bool IsSetupped => Handle != IGLObject.EMPTY_HANDLE;

        public Vector2i Size { get; private set; }

        public RBO(RenderBufferSettings settings, Vector2i size)
            : base(settings)
        {
            Size = size;
        }

        public override RBO Bind()
        {
            GL.BindRenderbuffer(Settings.RenderbufferTarget, Handle);
            return this;
        }

        public override RBO Setup()
        {
            GL.CreateRenderbuffers(1, out int handle);
            Handle = handle;
            if (Settings.IsMultiSample)
            {
                GL.NamedRenderbufferStorageMultisample(Handle, Settings.SampleCount, Settings.RenderbufferStorage, Size.X, Size.Y);
            }
            else
            {
                GL.NamedRenderbufferStorage(Handle, Settings.RenderbufferStorage, Size.X, Size.Y);
            }
            return this;
        }

        public override RBO Unbind()
        {
            GL.BindRenderbuffer(Settings.RenderbufferTarget, IGLObject.EMPTY_HANDLE);
            return this;
        }

        public override RBO SetLabel()
        {
            string label = $"RBO: ({Handle})";
            Logger.AddLog($"Setupped {label}");
            GL.ObjectLabel(ObjectLabelIdentifier.Renderbuffer, Handle, label.Length, label);
            return this;
        }

        public void Resize(Vector2i size)
        {
            Size = size;
            Dispose();
            Setup();
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
            GL.GetObjectLabel(ObjectLabelIdentifier.Buffer, Handle, 128, out int _, out string name);
            if (name.Length == 0)
            {
                name = $"RBO: ({Handle})";
            }
            Logger.AddLog($"Disposing {name}");
            GL.DeleteRenderbuffers(1, ref _handle);
            _handle = IGLObject.EMPTY_HANDLE;
        }
    }
}
