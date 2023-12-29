using OpenTK.Graphics.OpenGL4;

using RockEngine.Common.Utils;

namespace RockEngine.Rendering.OpenGL.Shaders
{
    [Flags]
    internal enum PipeLineSetupFlag
    {
        None = 0,
        VertexShader = 1,
        FragmentShader = 2,
        ComputeShader = 4,

        FullSetup = VertexShader & FragmentShader | ComputeShader

    }
    public sealed class Pipeline : ASetuppableGLObject
    {
        public RenderType RenderType;

        public readonly string Name;

        private PipeLineSetupFlag _setupFlag;

        public List<AShaderProgram> Shaders;

        public override bool IsSetupped => _setupFlag.HasFlag(PipeLineSetupFlag.FullSetup) && Handle != IGLObject.EMPTY_HANDLE;

        public Pipeline(RenderType renderType, string name)
        {
            Shaders = new List<AShaderProgram>();
            RenderType = renderType;
            Name = name;
        }

        public Pipeline UseShader(AShaderProgram shader, ProgramStageMask stageMask)
        {
            GL.UseProgramStages(Handle, stageMask, shader.Handle);
            Shaders.Add(shader);

            switch(stageMask)
            {
                case ProgramStageMask.VertexShaderBit:
                    _setupFlag |= PipeLineSetupFlag.VertexShader;
                    break;
                case ProgramStageMask.FragmentShaderBit:
                    _setupFlag |= PipeLineSetupFlag.FragmentShader;
                    break;
                case ProgramStageMask.ComputeShaderBit:
                    _setupFlag |= PipeLineSetupFlag.ComputeShader;
                    break;
            }
            return this;
        }

        public override Pipeline Bind()
        {
            GL.BindProgramPipeline(Handle);
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
            GL.GetObjectLabel(ObjectLabelIdentifier.Buffer, Handle, 64, out int length, out string name);
            if(name.Length == 0)
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
            GL.BindProgramPipeline(0);
            return this;
        }

        public override Pipeline Setup()
        {
            GL.CreateProgramPipelines(1, out _handle);
            return this;
        }

        public override bool IsBinded()
            => GL.GetInteger(GetPName.ProgramPipelineBinding) == Handle;
    }
}
