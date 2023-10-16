using OpenTK.Graphics.OpenGL4;

using RockEngine.Utils;

namespace RockEngine.OpenGL.Shaders
{
    internal sealed class Pipeline : ASetuppableGLObject
    {
        public static Pipeline CurrentPipeline;

        private static Pipeline _prevPipeline;

        public RenderType RenderType;

        public readonly string Name;

        public override bool IsSetupped => Handle != IGLObject.EMPTY_HANDLE;

        public Pipeline(RenderType renderType, string name)
        {
            RenderType = renderType;
            Name = name;
        }

        public Pipeline UseShader(AShaderProgram shader, ProgramStageMask stageMask)
        {
            GL.UseProgramStages(Handle, stageMask, shader.Handle);
            return this;
        }

        public override Pipeline Bind()
        {
            _prevPipeline = CurrentPipeline;
            CurrentPipeline = this;
            GL.BindProgramPipeline(Handle);
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
                name = $"Pipeline: ({Handle})";
            }
            Logger.AddLog($"Disposing {name}");
            GL.DeleteProgramPipeline(_handle);
            // now Handle is 0 
            _handle = IGLObject.EMPTY_HANDLE;

            _disposed = true;
        }


        public override Pipeline SetLabel()
        {
            string label = $"Pipeline: ({Handle})";
            Logger.AddLog($"Setupped {label}");

            GL.ObjectLabel(ObjectLabelIdentifier.ProgramPipeline, Handle, label.Length, label);
            return this;
        }

        public override Pipeline Unbind()
        {
            CurrentPipeline = _prevPipeline;
            GL.BindProgramPipeline(CurrentPipeline.Handle);
            return this;
        }

        public override Pipeline Setup()
        {
            GL.CreateProgramPipelines(1, out _handle);
            return this;
        }
    }
}
