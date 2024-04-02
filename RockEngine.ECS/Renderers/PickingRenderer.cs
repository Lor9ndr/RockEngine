using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SimdLinq;
using RockEngine.Common;
using RockEngine.Common.Utils;
using RockEngine.ECS;
using RockEngine.Rendering.OpenGL;
using RockEngine.Rendering.OpenGL.Buffers;
using RockEngine.Rendering.OpenGL.Buffers.UBOBuffers;
using RockEngine.Rendering.OpenGL.Settings;
using RockEngine.Rendering.OpenGL.Shaders;
using RockEngine.ECS.Assets;

namespace RockEngine.Rendering.Renderers
{
    public sealed class PickingRenderer : IRenderer<Scene>, IDisposable
    {
        private const BufferAccessMask _pickingBufferFlags = BufferAccessMask.MapWriteBit | BufferAccessMask.MapPersistentBit;
        private const int PICKING_BUFFER_LOCATION = 5;
        private const int PICKING_BUFFER_STRIDE = 16;

        private readonly PickingTexture _pickingTexture;
        private readonly AShaderProgram _pickingShader;
        private VBO _pickingBuffer;

        public PickingRenderer(Vector2i size)
        {
            var basePicking = Path.Combine(PathConstants.RESOURCES, PathConstants.SHADERS, PathConstants.DEBUG, PathConstants.PICKING);
            _pickingTexture = new PickingTexture(size);
            _pickingShader = ShaderProgram.GetOrCreate("PickingShader",
               new VertexShader(Path.Combine(basePicking, "Picking.vert")),
             new FragmentShader(Path.Combine(basePicking, "Picking.frag")));
            IRenderingContext.Update((context) =>
            {
                _pickingShader.Setup(context);
                _pickingBuffer = CreateInstanceBuffer(context,20000)
                .LockBuffer(context);

            });
        }

        public void Begin(IRenderingContext context)
        {
            _pickingShader.BindIfNotBinded(context);
            context.Disable(EnableCap.Blend);
            _pickingTexture.BeginWrite(context);
        }

        public void Render(IRenderingContext context, Scene item)
        {
            if(!_pickingBuffer.IsSetupped)
            {
                return;
            }
            foreach(var meshEntry in item.GetGroupedGameObjects())
            {
                Mesh mesh = meshEntry.Key;
                var materialGroups = meshEntry.Value;
                if(!mesh.IsSetupped)
                {
                    continue;
                }

                int totalObjects = 0;
                foreach(var group in materialGroups.Values)
                {
                    totalObjects += group.Count;
                }
                PickingData[] pickingDataArray = new PickingData[totalObjects]; // Prepare picking data array
                Dictionary<Material, int> materialStartIndices = new Dictionary<Material, int>(materialGroups.Count);

                int currentIndex = 0;
                foreach(var materialGroup in materialGroups)
                {
                    materialStartIndices[materialGroup.Key] = currentIndex;
                    foreach(var gameObject in materialGroup.Value)
                    {
                        pickingDataArray[currentIndex] = new PickingData(gameObject.GameObjectID); // Fill picking data
                        currentIndex++;
                    }
                }

                // Send picking data to the GPU
                _pickingBuffer
                    .WaitBuffer(context)
                    .Bind(context)
                    .SendDataMappedBuffer(context, pickingDataArray, 0, 0, pickingDataArray.Length);

                foreach(var materialGroup in materialGroups)
                {
                    materialGroup.Key.SendData(context);
                    mesh.InstanceCount = materialGroup.Value.Count;

                    int startIndex = materialStartIndices[materialGroup.Key];
                    // Adjust instance attributes for the current material group
                    mesh.AdjustInstanceAttributesForGroup(context, startIndex);
                    AdjustInstanceAttributesForGroup(context, startIndex, mesh.VAO);
                    mesh.Render(context);
                }
            }
        }

        public void Render(IRenderingContext context, GameObject item)
        {
            var mesh = item.GetComponent<MeshComponent>();
            mesh.Mesh.VAO.Bind(context);
            _pickingBuffer.Bind(context);
            SetInstancedAttributes(context, mesh.Mesh.VAO);

            PickingData[] pickingDataArray = [new PickingData(item.GameObjectID)];

            _pickingBuffer
               .WaitBuffer(context)
               .Bind(context)
               .SendDataMappedBuffer(context, pickingDataArray, 0, 0, pickingDataArray.Length* sizeof(uint));
            mesh.Mesh.PrepareSendingModel(context, [item.Transform.GetModelMatrix()],0,1);
            mesh.Mesh.AdjustInstanceAttributesForGroup(context,0);
            mesh.Mesh.Render(context);
        }

        public void End(IRenderingContext context)
        {
            _pickingTexture.EndWrite(context);
            _pickingShader.Unbind(context);
        }

        public void ResizeTexture(Vector2i size)
        {
            _pickingTexture.CheckSize(size);
        }

        public void ReadPixel(IRenderingContext context, int x, int y, ref PixelInfo pixelInfo)
        {
            _pickingTexture.ReadPixel(context, x, y, ref pixelInfo);
        }

        public void Update(Scene item)
        {
            throw new NotImplementedException();
        }

        private VBO CreateInstanceBuffer(IRenderingContext context, int size)
        {
            var vbo = new VBO(BufferSettings.DefaultVBOSettings with { BufferSize = size, BufferUsageHint = BufferUsageHint.StreamDraw })
                             .Setup(context)
                             .SetLabel(context)
                             .SetupBufferStorage(context, BufferStorageFlags.MapPersistentBit | BufferStorageFlags.MapWriteBit, 0)
                             .Bind(context)
                             .MapBuffer(context, _pickingBufferFlags, out _);
            return vbo;
        }

        private void SetInstancedAttributes(IRenderingContext context,VAO vao)
        {
            context.VertexArrayVertexBuffer(vao!.Handle, PICKING_BUFFER_LOCATION, _pickingBuffer.Handle, nint.Zero, PICKING_BUFFER_STRIDE);

            for(int i = 0; i < 1; i++)
            {
                context
                    .EnableVertexArrayAttrib(vao.Handle, PICKING_BUFFER_LOCATION + i)
                    .VertexAttribPointer(PICKING_BUFFER_LOCATION + i, 1, VertexAttribPointerType.UnsignedInt, false, PICKING_BUFFER_STRIDE, sizeof(uint) * i) 
                    .VertexAttribDivisor(PICKING_BUFFER_LOCATION + i, 1);
            }
        }
        private void AdjustInstanceAttributesForGroup(IRenderingContext context, int startIndex, VAO vao)
        {
            int baseOffset = startIndex * 1 * sizeof(uint); 

            // Assuming you've already bound the VAO and VBO relevant to this operation.
            vao.Bind(context);
            _pickingBuffer.Bind(context);
            for(int i = 0; i < 1; i++)
            {
                // Enable the vertex attribute array for each row of the matrix
                context.EnableVertexArrayAttrib(vao.Handle, PICKING_BUFFER_LOCATION + i);

                // Set the vertex attribute pointer to point at the correct part of the buffer
                context.VertexAttribPointer(PICKING_BUFFER_LOCATION + i, 1, VertexAttribPointerType.UnsignedInt, false, sizeof(uint), baseOffset + i);

                // Tell OpenGL this attribute is per-instance (divisor 1) rather than per-vertex (divisor 0)
                context.VertexAttribDivisor(PICKING_BUFFER_LOCATION + i, 1);
            }
        }

        public void Dispose()
        {
             _pickingTexture.Dispose();
        }
    }
}
