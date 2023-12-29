using OpenTK.Graphics.OpenGL4;

using RockEngine.Common.Vertices;
using RockEngine.ECS.Assets;
using RockEngine.Rendering.OpenGL.Buffers;
using RockEngine.Rendering.OpenGL.Settings;

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

        public Mesh SetupMeshVertices(ref Vertex3D[ ] vertices)
        {
            Vertices = vertices;
            VAO = new VAO()
                .Setup()
                .Bind()
                .SetLabel();

            VBO = new VBO(BufferSettings.DefaultVBOSettings with { BufferSize = Vertex3D.Size * vertices.Length })
                .Setup()
                .Bind()
                .SendData(vertices)
                .SetLabel();

            VAO.EnableVertexArrayAttrib(0)
                .VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex3D.Size, Vertex3D.PositionOffset)
                .EnableVertexArrayAttrib(1)
                .VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Vertex3D.Size, Vertex3D.NormalOffset)
                .EnableVertexArrayAttrib(2)
                .VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, Vertex3D.Size, Vertex3D.TexCoordsOffset);
            //PrepareSendingModel();
            VAO.Unbind();
            return this;
        }

        public Mesh SetupMeshIndicesVertices(ref int[ ] indices, ref Vertex3D[ ] vertices)
        {
            Vertices = vertices;
            Indices = indices;

            VAO = new VAO()
                .Setup()
                .Bind()
                .SetLabel();

            VBO = new VBO(BufferSettings.DefaultVBOSettings with { BufferSize = Vertex3D.Size * vertices.Length })
                .Setup()
                .Bind()
                .SendData(in vertices)
                .SetLabel();

            EBO = new EBO(new BufferSettings(sizeof(int) * indices.Length, BufferUsageHint.StaticDraw))
                .Setup()
                .SendData(in indices)
                .Bind()
                .SetLabel();

            VAO.EnableVertexArrayAttrib(0)
                .VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex3D.Size, Vertex3D.PositionOffset)
                .EnableVertexArrayAttrib(1)
                .VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Vertex3D.Size, Vertex3D.NormalOffset)
                .EnableVertexArrayAttrib(2)
                .VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, Vertex3D.Size, Vertex3D.TexCoordsOffset);

            //PrepareSendingModel();

            VBO.Unbind();
            EBO.Unbind();
            VAO.Unbind();
            return this;
        }

        public Mesh SetupMeshVertices()
        {
            SetupMeshVertices(ref Vertices!);
            return this;
        }

        public Mesh SetupMeshIndicesVertices()
        {
            SetupMeshIndicesVertices(ref Indices!, ref Vertices!);
            return this;
        }

        public override void Render()
        {
            //var command = new DrawMeshCommand(this);
            VAO!.BindIfNotBinded();

            if(InstanceCount > 1)
            {
                //TODO: INSTANCE RENDERING 
                //THINK ABOUT MATERIALS AND OTHER COMPONENTS ATTACHED TO THE GameObject
                if(HasIndices)
                {
                    EBO!.BindIfNotBinded();
                    GL.DrawElementsInstanced(PrimitiveType, Indices!.Length, DrawElementsType.UnsignedInt, nint.Zero, InstanceCount);
                }
                else
                {
                    GL.DrawArraysInstanced(PrimitiveType, 0, Vertices!.Length, InstanceCount);
                }
            }
            else
            {
                if(HasIndices)
                {
                    EBO!.BindIfNotBinded();
                    GL.DrawElements(PrimitiveType, Indices!.Length, DrawElementsType.UnsignedInt, 0);
                }
                else
                {
                    GL.DrawArrays(PrimitiveType, 0, Vertices!.Length);
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

        private void CreateInstanceBuffer(int size)
        {
            ModelBuffer = new VBO(BufferSettings.DefaultVBOSettings with { BufferSize = size, BufferUsageHint = BufferUsageHint.StreamDraw })
                             .Setup()
                             .SetLabel()
                             .SetupBufferStorage(BufferStorageFlags.MapPersistentBit | BufferStorageFlags.MapWriteBit, 0)
                             .Bind()
                             .MapBuffer(BufferAccessMask.MapPersistentBit | BufferAccessMask.MapWriteBit, out _mappedBuffer);

            GL.VertexArrayVertexBuffer(VAO!.Handle, VAO.INSTANCE_MODELS_ATTRIBUTE, ModelBuffer.Handle, nint.Zero, sizeof(float) * 4 * 4);
        }

        private void SetInstancedAttributes()
        {
            var mat4Size = sizeof(float) * 4 * 4;
            for(int i = 0; i < 4; i++)
            {
                VAO!.EnableVertexArrayAttrib(VAO.INSTANCE_MODELS_ATTRIBUTE + i);
                VAO.VertexAttribPointer(VAO.INSTANCE_MODELS_ATTRIBUTE + i, 4, VertexAttribPointerType.Float, false, mat4Size, sizeof(float) * i * 4);
                VAO.VertexAttribDivisor(VAO.INSTANCE_MODELS_ATTRIBUTE + i, 1);
            }
        }

        public void Dispose()
        {
            EBO?.Dispose();
            ModelBuffer?.UnmapBuffer(ref _mappedBuffer);
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
