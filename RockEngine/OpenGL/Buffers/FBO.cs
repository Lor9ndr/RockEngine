using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.OpenGL.Settings;
using RockEngine.OpenGL.Textures;
using RockEngine.Utils;

namespace RockEngine.OpenGL.Buffers
{
    internal class FBO : ASetuppableGLObject<FrameBufferSettings>
    {
        public Vector2i Size { get; protected set; }
        protected List<Texture> _textures;


        public FBO(FrameBufferSettings settings, Vector2i size, params Texture[] textures)
            : base(settings)
        {
            Size = size;
            _textures = new List<Texture>();
            _textures.AddRange(textures);
        }

        public override bool IsSetupped => Handle != IGLObject.EMPTY_HANDLE;



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
            GL.GetObjectLabel(ObjectLabelIdentifier.Framebuffer, Handle, 64, out int length, out string name);
            if (name.Length == 0)
            {
                name = $"FBO: ({Handle})";
            }
            Logger.AddLog($"Disposing {name}");
            GL.DeleteFramebuffer(_handle);
            // now Handle is 0 
            _handle = IGLObject.EMPTY_HANDLE;
        }

        public override FBO SetLabel()
        {
            string label = $"FBO: ({Handle})";
            Logger.AddLog($"Setupped {label}");
            GL.ObjectLabel(ObjectLabelIdentifier.Framebuffer, Handle, label.Length, label);
            return this;
        }

        public override FBO Setup()
        {
            GL.CreateFramebuffers(1, out int handle);
            Handle = handle;

            SetupInternal();
            return this;
        }

        protected virtual FBO SetupInternal()
        {
            foreach (var texture in _textures)
            {
                texture.Resize(Size);
                if (!texture.IsSetupped)
                {
                    texture
                        .Setup()
                        .SetLabel();
                }
                GL.NamedFramebufferTexture(Handle, texture.Settings.FramebufferAttachment, texture.Handle, 0);
            }

            CheckBuffer();
            return this;
        }

        public virtual void Resize(Vector2i size)
        {
            Size = size;
            SetupInternal();
        }
        public void CheckBuffer()
        {
            var status = GL.CheckNamedFramebufferStatus(Handle, Settings.FramebufferTarget);
            if (status != FramebufferStatus.FramebufferComplete)
            {
                Logger.AddError($"Can't create a framebuffer. Status : {status}");
            }
        }

        public override FBO Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, Handle);
            return this;
        }

        public override FBO Unbind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, IGLObject.EMPTY_HANDLE);
            return this;
        }
    }
}
