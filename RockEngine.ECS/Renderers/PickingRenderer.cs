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
                _pickingBuffer = CreateInstanceBuffer(context,4);

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
            var groupedData = item.GetGroupedGameObjects();
            int totalObjects = groupedData.Sum(meshGroup => meshGroup.Value.Sum(matGroup => matGroup.Value.Count));
            PickingData[] pickingDataArray = new PickingData[totalObjects];
            int globalIndex = 0;
            int drawID = 0;

            foreach(var meshGroup in groupedData)
            {
                var mesh = meshGroup.Key;
                if(!mesh.IsSetupped)
                {
                    continue;
                }
                int instanceID = 0;
                int instancesCount = meshGroup.Value.Sum(matGroup => matGroup.Value.Count);
                for(int i = 0; i < instancesCount; i++)
                {
                    pickingDataArray[globalIndex++] = new PickingData((uint)instanceID++, (uint)drawID);
                }
                drawID++;

                mesh.VAO.Bind(context);
                SetInstancedAttributes(context, mesh.VAO);
            }
            _pickingBuffer.SendData(context, pickingDataArray);
            foreach(var meshGroup in groupedData)
            {
                var mesh = meshGroup.Key;
                if(!mesh.IsSetupped)
                {
                    continue;
                }
                mesh.Render(context);
            }
        }

        public void Render(IRenderingContext context, GameObject item)
        {
            var mesh = item.GetComponent<MeshComponent>();
            mesh.Mesh.VAO.Bind(context);
            _pickingBuffer.Bind(context);
            SetInstancedAttributes(context, mesh.Mesh.VAO);

            PickingData[] pickingDataArray = [new PickingData(1, item.GameObjectID)];

            _pickingBuffer.SendData(context, pickingDataArray);
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
                             .Bind(context);
            return vbo;
        }

        private void SetInstancedAttributes(IRenderingContext context,VAO vao)
        {
            for(int i = 0; i < 2; i++)
            {
                context
                    .EnableVertexArrayAttrib(vao.Handle, PICKING_BUFFER_LOCATION + i)
                    .VertexAttribPointer(PICKING_BUFFER_LOCATION + i, 1, VertexAttribPointerType.UnsignedInt, false, PICKING_BUFFER_STRIDE, sizeof(uint) * i) 
                    .VertexAttribDivisor(PICKING_BUFFER_LOCATION + i, 1);
            }
        }

        public void Dispose()
        {
             _pickingTexture.Dispose();
        }
    }
}
