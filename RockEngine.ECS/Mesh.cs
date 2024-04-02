using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.Common.Vertices;
using RockEngine.ECS.Assets;
using RockEngine.Rendering;
using RockEngine.Rendering.OpenGL.Buffers;
using RockEngine.Rendering.OpenGL.Settings;

using System.Text.Json.Serialization;

namespace RockEngine.ECS
{
    public class Mesh : ABaseMesh, IDisposable
    {
        private const BufferAccessMask _modelFlags =
            BufferAccessMask.MapPersistentBit
            | BufferAccessMask.MapWriteBit;
        private const int MATRIX_SIZE = 64;


        public VBO ModelBuffer { get; private set;}

        public VAO VAO { get; private set; }
        public VBO VBO { get; private set; }
        public EBO EBO { get; private set; }

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

        public Mesh SetupMeshVertices(IRenderingContext context, Vertex3D[] vertices)
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
            VAO!.BindIfNotBinded(context);
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

        public void PrepareSendingModel(IRenderingContext context, Matrix4[] transforms, int startIndex, int count)
        {
            if(!IsSetupped)
            {
                return;
            }
            VAO.Bind(context);

            if(startIndex < 0 || count < 0 || startIndex + count > transforms.Length)
            {
                throw new ArgumentOutOfRangeException("startIndex or count is out of the bounds of the transforms array.");
            }

            var size = count * MATRIX_SIZE; // 64 bytes per matrix
            int requiredBufferSize = transforms.Length * MATRIX_SIZE;

            if(ModelBuffer == null || !ModelBuffer.IsSetupped || requiredBufferSize > ModelBuffer.Settings.BufferSize)
            {
                SetupInstanceBuffer(context, requiredBufferSize);
                ModelBuffer!.LockBuffer(context);
            }

            var mappedBuffer = ModelBuffer!.GetMappedBuffer();
            if(mappedBuffer == nint.Zero)
            {
                ModelBuffer.MapBuffer(context, _modelFlags,out _);
                ModelBuffer!.LockBuffer(context);

            }
            int byteOffset = startIndex * MATRIX_SIZE;

            if(byteOffset + size > ModelBuffer.Settings.BufferSize)
            {
                throw new InvalidOperationException("The calculated byte offset and size exceed the buffer's capacity.");
            }

            ModelBuffer
                .WaitBuffer(context)
                .Bind(context)
                .SendDataMappedBuffer(context, transforms, startIndex, byteOffset, size);

            ModelBuffer.Unbind(context);
            VAO!.Unbind(context);
        }

        private void SetupInstanceBuffer(IRenderingContext context, int size)
        {
            ModelBuffer = CreateInstanceBuffer(context, size);
            ModelBuffer!.Bind(context);
            SetInstancedAttributes(context);
        }

        private unsafe VBO CreateInstanceBuffer(IRenderingContext context, int size)
        {
            var vbo = new VBO(BufferSettings.DefaultVBOSettings with { BufferSize = size, BufferUsageHint = BufferUsageHint.StreamDraw })
                             .Setup(context)
                             .SetLabel(context)
                             .SetupBufferStorage(context, BufferStorageFlags.MapPersistentBit | BufferStorageFlags.MapWriteBit, 0)
                             .Bind(context)
                             .MapBuffer(context, _modelFlags, out _);
            context.VertexArrayVertexBuffer(VAO!.Handle, VAO.INSTANCE_MODELS_ATTRIBUTE, vbo.Handle, nint.Zero, MATRIX_SIZE); // 64 per Matrix
            return vbo;
        }
       

        private void SetInstancedAttributes(IRenderingContext context)
        {
            for(int i = 0; i < 4; i++)
            {
                context.EnableVertexArrayAttrib(VAO.Handle,VAO.INSTANCE_MODELS_ATTRIBUTE + i)
                    .VertexAttribPointer(VAO.INSTANCE_MODELS_ATTRIBUTE + i, 4, VertexAttribPointerType.Float, false, MATRIX_SIZE, 16 * i) // 16 per Vertex4 in matrix4
                    .VertexAttribDivisor(VAO.INSTANCE_MODELS_ATTRIBUTE + i, 1);
            }
        }
        public void AdjustInstanceAttributesForGroup(IRenderingContext context, int startIndex)
        {
            int matrixSize = 4; // Number of vec4s in a mat4.
            int floatSize = sizeof(float);
            int vec4Size = 4 * floatSize;
            int baseOffset = startIndex * 16 * floatSize; // 16 floats in a 4x4 matrix, calculate byte offset.

            // Assuming you've already bound the VAO and VBO relevant to this operation.
            VAO.Bind(context);
            ModelBuffer.Bind(context);
            for(int i = 0; i < matrixSize; i++)
            {
                // Enable the vertex attribute array for each row of the matrix
                context.EnableVertexArrayAttrib(VAO.Handle, VAO.INSTANCE_MODELS_ATTRIBUTE + i);

                // Set the vertex attribute pointer to point at the correct part of the buffer
                context.VertexAttribPointer(VAO.INSTANCE_MODELS_ATTRIBUTE + i, 4, VertexAttribPointerType.Float, false, MATRIX_SIZE, baseOffset + i * vec4Size);

                // Tell OpenGL this attribute is per-instance (divisor 1) rather than per-vertex (divisor 0)
                context.VertexAttribDivisor(VAO.INSTANCE_MODELS_ATTRIBUTE + i, 1);
            }
        }

        public void Dispose()
        {
            EBO?.Dispose();
            IRenderingContext.Update(context =>
            {
                    ModelBuffer?.UnmapBuffer(context);
            });

            VBO?.Dispose();
            VAO?.Dispose();
        }

        public void OnUpdate()
        {
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
