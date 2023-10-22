using Newtonsoft.Json;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.Engine.ECS.GameObjects;
using RockEngine.OpenGL.Buffers;
using RockEngine.OpenGL.Settings;
using RockEngine.OpenGL.Vertices;

using RockEngine.OpenGL.Buffers.UBOBuffers;

namespace RockEngine.Engine.ECS
{
    public class MeshComponent : ABaseMesh, IComponent, IDisposable
    {
        private VAO? _vao;
        private VBO? _vbo;

        private EBO? _ebo;
        private VBO? _modelBuffer;

        private nint _mappedBuffer;

        [JsonIgnore]
        public GameObject? Parent { get; set; }

        [JsonIgnore]
        public int Order => 999;

        [JsonIgnore]
        public bool IsSetupped => _vao is not null && _vao.IsSetupped;

        public MeshComponent(ref Vertex3D[] vertices, ref int[] indices, string name, string path, Guid id)
            : base(ref indices, ref vertices, name, path, id)
        {
        }

        public MeshComponent(ref Vertex3D[] vertices, string name, string path, Guid id)
           : base(ref vertices, name, path, id)
        {
        }

        public MeshComponent SetupMeshVertices(ref Vertex3D[] vertices)
        {
            Vertices = vertices;
            _vao = new VAO()
                .Setup()
                .Bind()
                .SetLabel();

            _vbo = new VBO(BufferSettings.DefaultVBOSettings with { BufferSize = Vertex3D.Size * vertices.Length })
                .Setup()
                .Bind()
                .SendData(vertices)
                .SetLabel();

            _vao.EnableVertexArrayAttrib(0)
                .VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex3D.Size, Vertex3D.PositionOffset)
                .EnableVertexArrayAttrib(1)
                .VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Vertex3D.Size, Vertex3D.NormalOffset)
                .EnableVertexArrayAttrib(2)
                .VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, Vertex3D.Size, Vertex3D.TexCoordsOffset);
            //PrepareSendingModel();
            _vao.Unbind();
            return this;
        }

        public MeshComponent SetupMeshIndicesVertices(ref int[] indices, ref Vertex3D[] vertices)
        {
            Vertices = vertices;
            Indices = indices;

            _vao = new VAO()
                .Setup()
                .Bind()
                .SetLabel();

            _vbo = new VBO(BufferSettings.DefaultVBOSettings with { BufferSize = Vertex3D.Size * vertices.Length })
                .Setup()
                .Bind()
                .SendData(in vertices)
                .SetLabel();

            _ebo = new EBO(new BufferSettings(sizeof(int) * indices.Length, BufferUsageHint.StaticDraw))
                .Setup()
                .SendData(in indices)
                .Bind()
                .SetLabel();

            _vao.EnableVertexArrayAttrib(0)
                .VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex3D.Size, Vertex3D.PositionOffset)
                .EnableVertexArrayAttrib(1)
                .VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Vertex3D.Size, Vertex3D.NormalOffset)
                .EnableVertexArrayAttrib(2)
                .VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, Vertex3D.Size, Vertex3D.TexCoordsOffset);

            //PrepareSendingModel();

            _vbo.Unbind();
            _ebo.Unbind();
            _vao.Unbind();
            return this;
        }

        public override void RenderOnEditorLayer()
        {
            Render();
        }

        public override void Render()
        {

            if (!IsSetupped)
            {
                OnStart();
            }

            _vao!.Bind();

            if (InstanceCount > 1)
            {
                //TODO: INSTANCE RENDERING 
                //THINK ABOUT MATERIALS AND OTHER COMPONENTS ATTACHED TO THE GameObject
                if (HasIndices)
                {
                    _ebo!.Bind();
                    GL.DrawElementsInstanced(PrimitiveType.Triangles, Indices!.Length, DrawElementsType.UnsignedInt, IntPtr.Zero, InstanceCount);
                }
                else
                {
                    GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, Vertices!.Length, InstanceCount);
                }
            }
            else
            {
                if (HasIndices)
                {
                    _ebo!.Bind();
                    GL.DrawElements(PrimitiveType.Triangles, Indices!.Length, DrawElementsType.UnsignedInt, 0);
                }
                else
                {
                    GL.DrawArrays(PrimitiveType.Triangles, 0, Vertices!.Length);
                }
            }

            _vao.Unbind();
        }

        private void PrepareSendingModel()
        {
            Models ??= new Matrix4[1] { Parent!.Transform.GetModelMatrix() };

            var size = Models.Length * sizeof(float) * 4 * 4;
            if (_modelBuffer is null || !_modelBuffer.IsSetupped || size != _modelBuffer.Settings.BufferSize)
            {
                _modelBuffer?.Dispose();
                CreateInstanceBuffer(size);
                _modelBuffer!.Bind();
                SetInstancedAttributes();
            }
            if (_mappedBuffer == IntPtr.Zero)
            {
                _modelBuffer.MapBuffer(BufferAccessMask.MapPersistentBit | BufferAccessMask.MapWriteBit, out _mappedBuffer);
            }
        }

        private void CreateInstanceBuffer(int size)
        {
            _modelBuffer = new VBO(BufferSettings.DefaultVBOSettings with { BufferSize = size, BufferUsageHint = BufferUsageHint.StreamDraw })
                             .Setup()
                             .SetLabel()
                             .SetupBufferStorage(BufferStorageFlags.MapPersistentBit | BufferStorageFlags.MapWriteBit, 0)
                             .Bind()
                             .MapBuffer(BufferAccessMask.MapPersistentBit | BufferAccessMask.MapWriteBit, out _mappedBuffer);

            GL.VertexArrayVertexBuffer(_vao!.Handle, VAO.INSTANCE_MODELS_ATTRIBUTE, _modelBuffer.Handle, IntPtr.Zero, sizeof(float) * 4 * 4);
        }

        private void SetInstancedAttributes()
        {
            var mat4Size = sizeof(float) * 4 * 4;
            for (int i = 0; i < 4; i++)
            {
                _vao!.EnableVertexArrayAttrib(VAO.INSTANCE_MODELS_ATTRIBUTE + i);
                _vao.VertexAttribPointer(VAO.INSTANCE_MODELS_ATTRIBUTE + i, 4, VertexAttribPointerType.Float, false, mat4Size, sizeof(float) * i * 4);
                _vao.VertexAttribDivisor(VAO.INSTANCE_MODELS_ATTRIBUTE + i, 1);
            }
        }

        public void Dispose()
        {
            _ebo?.Dispose();
            _modelBuffer?.UnmapBuffer(ref _mappedBuffer);
            _modelBuffer?.Dispose();
            _vbo?.Dispose();
            _vao?.Dispose();
        }


        public void OnStart()
        {
            if (!IsSetupped)
            {
                if (HasIndices)
                {
                    SetupMeshIndicesVertices(ref Indices!, ref Vertices!);
                }
                else
                {
                    SetupMeshVertices(ref Vertices!);
                }
            }
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
        ~MeshComponent()
        {
            Dispose();
        }
    }
}
