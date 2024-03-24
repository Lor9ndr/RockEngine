using OpenTK.Graphics.OpenGL4;

using RockEngine.Common;
using RockEngine.Common.Vertices;
using RockEngine.ECS.Assets;
using RockEngine.Rendering;
using RockEngine.Rendering.OpenGL.Buffers;
using RockEngine.Rendering.OpenGL.Settings;

using System.ComponentModel;
using System.Text.Json.Serialization;

namespace RockEngine.ECS
{
    public class Mesh : ABaseMesh, IDisposable
    {
        public VAO VAO { get; private set; }
        public VBO VBO { get; private set; }
        public EBO EBO { get; private set; }
        public VBO ModelBuffer { get; private set; }

        private nint _mappedBuffer;

        public bool IsSetupped => VAO is not null && VAO.IsSetupped;

        public Mesh(ref Vertex3D[ ] vertices, ref int[ ] indices, string name, string path, Guid id)
            : base(ref indices, ref vertices, name, path, id)
        {
        }

        public Mesh(ref Vertex3D[ ] vertices, string name, string path, Guid id)
           : base(ref vertices, name, path, id)
        {
            VAO = new VAO();
        }

        [JsonConstructor]
        public Mesh() : base() => VAO = new VAO();

        public Mesh SetupMeshVertices(IRenderingContext context, Vertex3D[ ] vertices)
        {
            Vertices = vertices;
            VAO = new VAO()
           .Setup(context)
           .Bind(context)
           .SetLabel(context);

            VBO = new VBO(BufferSettings.DefaultVBOSettings with { BufferSize = Vertex3D.Size * vertices.Length })
                .Setup(context)
                .Bind(context)
                .SendData(context, vertices)
                .SetLabel(context);

            context.EnableVertexArrayAttrib(VAO.Handle, 0)
                .VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex3D.Size, Vertex3D.PositionOffset)
                .EnableVertexArrayAttrib(VAO.Handle, 1)
                .VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Vertex3D.Size, Vertex3D.NormalOffset)
                .EnableVertexArrayAttrib(VAO.Handle, 2)
                .VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, Vertex3D.Size, Vertex3D.TexCoordsOffset);
            //PrepareSendingModel();
            VAO.Unbind(context);

            return this;
        }

        public Mesh SetupMeshIndicesVertices(IRenderingContext context, int[] indices, Vertex3D[] vertices)
        {
            Vertices = vertices;
            Indices = indices;
            VAO = new VAO()
            .Setup(context)
            .Bind(context)
            .SetLabel(context);

            VBO = new VBO(BufferSettings.DefaultVBOSettings with { BufferSize = Vertex3D.Size * vertices.Length })
                .Setup(context)
                .Bind(context)
                .SendData(context, vertices)
                .SetLabel(context);

            EBO = new EBO(new BufferSettings(sizeof(int) * indices.Length, BufferUsageHint.StaticDraw))
                .Setup(context)
                .SendData(context, indices)
                .Bind(context)
                .SetLabel(context);

            context.EnableVertexArrayAttrib(VAO.Handle, 0)
                .VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex3D.Size, Vertex3D.PositionOffset)
                .EnableVertexArrayAttrib(VAO.Handle, 1)
                .VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Vertex3D.Size, Vertex3D.NormalOffset)
                .EnableVertexArrayAttrib(VAO.Handle, 2)
                .VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, Vertex3D.Size, Vertex3D.TexCoordsOffset);

            //PrepareSendingModel();

            VBO.Unbind(context);
            EBO.Unbind(context);
            VAO.Unbind(context);
            return this;
        }

        public Mesh SetupMeshVertices(IRenderingContext context)
        {
            SetupMeshVertices(context, Vertices!);
            return this;
        }

        public Mesh SetupMeshIndicesVertices(IRenderingContext context)
        {
            SetupMeshIndicesVertices(context, Indices!, Vertices!);
            return this;
        }

        public override void Render(IRenderingContext context)
        {
            //var command = new DrawMeshCommand(this);
            VAO!.BindIfNotBinded(context);

            if(InstanceCount > 1)
            {
                //TODO: INSTANCE RENDERING 
                //THINK ABOUT MATERIALS AND OTHER COMPONENTS ATTACHED TO THE GameObject
                if(HasIndices)
                {
                    EBO!.BindIfNotBinded(context);
                    context.DrawElementsInstanced(PrimitiveType, Indices!.Length, DrawElementsType.UnsignedInt, nint.Zero, InstanceCount);
                }
                else
                {
                    context.DrawArraysInstanced(PrimitiveType, 0, Vertices!.Length, InstanceCount);
                }
            }
            else
            {
                if(HasIndices)
                {
                    EBO!.BindIfNotBinded(context);
                    context.DrawElements(PrimitiveType, Indices!.Length, DrawElementsType.UnsignedInt, 0);
                }
                else
                {
                    context.DrawArrays(PrimitiveType, 0, Vertices!.Length);
                }
            }
        }

        private void PrepareSendingModel()
        {
            /* Models ??= new Matrix4[1] { Parent!.Transform.GetModelMatrix() };

             var size = Models.Length * sizeof(float) * 4 * 4;
             if(_modelBuffer is null || !_modelBuffer.IsSetupped || size != _modelBuffer.Settings.BufferSize)
             {
                 _modelBuffer?.Dispose();
                 CreateInstanceBuffer(size);
                 _modelBuffer!.Bind();
                 SetInstancedAttributes();
             }
             if(_mappedBuffer == nint.Zero)
             {
                 _modelBuffer.MapBuffer(BufferAccessMask.MapPersistentBit | BufferAccessMask.MapWriteBit, out _mappedBuffer);
             }*/
        }

        private void CreateInstanceBuffer(IRenderingContext context,int size)
        {
            ModelBuffer = new VBO(BufferSettings.DefaultVBOSettings with { BufferSize = size, BufferUsageHint = BufferUsageHint.StreamDraw })
                             .Setup(context)
                             .SetLabel(context)
                             .SetupBufferStorage(context, BufferStorageFlags.MapPersistentBit | BufferStorageFlags.MapWriteBit, 0)
                             .Bind(context)
                             .MapBuffer(context, BufferAccessMask.MapPersistentBit | BufferAccessMask.MapWriteBit, out _mappedBuffer);
            context.VertexArrayVertexBuffer(VAO!.Handle, VAO.INSTANCE_MODELS_ATTRIBUTE, ModelBuffer.Handle, nint.Zero, sizeof(float) * 4 * 4);
        }

        private void SetInstancedAttributes(IRenderingContext context)
        {
            var mat4Size = sizeof(float) * 4 * 4;
            for(int i = 0; i < 4; i++)
            {
                context.EnableVertexArrayAttrib(VAO.Handle,VAO.INSTANCE_MODELS_ATTRIBUTE + i)
                    .VertexAttribPointer(VAO.INSTANCE_MODELS_ATTRIBUTE + i, 4, VertexAttribPointerType.Float, false, mat4Size, sizeof(float) * i * 4)
                    .VertexAttribDivisor(VAO.INSTANCE_MODELS_ATTRIBUTE + i, 1);
            }
        }

        public void Dispose()
        {
            EBO?.Dispose();
            IRenderingContext.Update(context =>
            {
                ModelBuffer?.UnmapBuffer(context,ref _mappedBuffer);
            });

            ModelBuffer?.Dispose();
            VBO?.Dispose();
            VAO?.Dispose();
        }

        public void OnUpdate()
        {
            /*if (Models is null)
            {
                Models = new Matrix4[Transforms.Count];
                for (int i = 0; i < Transforms.Count; i++)
                {
                    Models[i] = Transforms[i].GetModelMatrix();
                }
            }
            InstanceCount = Models.Length;
            _vao?.Bind();

            PrepareSendingModel();

            _modelBuffer!
                .Bind()
                .WaitBuffer()
                .SendData(Models, _mappedBuffer)
                .LockBuffer();

            _modelBuffer.Unbind();
            _vao.Unbind();
            Models = null;*/
        }

        public void OnDestroy()
        {
            Dispose();
        }
        ~Mesh()
        {
            Dispose();
        }
    }
}
