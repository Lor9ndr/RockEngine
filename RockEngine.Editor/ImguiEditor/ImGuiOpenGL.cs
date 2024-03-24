using ImGuiNET;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.Common;
using RockEngine.ECS;
using RockEngine.Rendering;
using RockEngine.Rendering.OpenGL.Buffers;
using RockEngine.Rendering.OpenGL.Settings;
using RockEngine.Rendering.OpenGL.Shaders;
using RockEngine.Rendering.OpenGL.Textures;

using SkiaSharp;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RockEngine.Editor.ImguiEditor
{
    public class ImGuiOpenGL : IDisposable
    {
        /// <summary>
        /// Create a new instance.
        /// </summary>
        public ImGuiOpenGL()
        {
            AssureContextCreated();

            var io = ImGui.GetIO();
            io.DisplaySize = System.Numerics.Vector2.Zero;
            io.Fonts.AddFontDefault();
            io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;
            io.DisplayFramebufferScale = System.Numerics.Vector2.One;
            IRenderingContext.Update(CreateDeviceResources);
        }

        private static void AssureContextCreated()
        {
            nint context = ImGui.GetCurrentContext();
            if(context == nint.Zero)
            {
                context = ImGui.CreateContext();
                ImGui.SetCurrentContext(context);
            }
        }

        /// <summary>
        /// Load a TTF font and use it for rendering the GUI
        /// </summary>
        /// <param name="fontData">TTF file read into a byte array</param>
        /// <param name="sizePixels">Intented size in pixels. Bigger means bigger texture is created.</param>
        public void LoadFontTTF(byte[ ] fontData, float sizePixels)
        {
            var fonts = ImGui.GetIO().Fonts;
            fonts.Clear(); // replace existing fonts
            GCHandle pinnedArray = GCHandle.Alloc(fontData, GCHandleType.Pinned);
            nint pointer = pinnedArray.AddrOfPinnedObject();
            fonts.AddFontFromMemoryTTF(pointer, fontData.Length, sizePixels);
            pinnedArray.Free();
            IRenderingContext.Update(RecreateFontDeviceTexture);
            //ImGui.PushFont(fnt); // call only after ImGui.NewFrame()
        }

        /// <summary>
        /// Render the user interface
        /// </summary>
        /// <param name="windowResolution">Window resolution in pixels.</param>
        public void Render(IRenderingContext context,Vector2i windowResolution)
        {
            ImGui.GetIO().DisplaySize = new System.Numerics.Vector2(windowResolution.X, windowResolution.Y); // divide by DisplayFramebufferScale if necessary
            ImGui.Render();
            RenderImDrawData(context, ImGui.GetDrawData());
        }

        /// <summary>
        /// Dispose all OpenGL resources
        /// </summary>
        public void Dispose()
        {
            vertexArray.Dispose();
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();

            fontTexture.Dispose();
            _shader.Dispose();
            GC.SuppressFinalize(this);
        }

        Texture fontTexture;

        private AShaderProgram _shader;
        private int _shaderFontTextureLocation;
        private int _shaderProjectionMatrixLocation;

        readonly VAO vertexArray = new VAO().Setup(IRenderingContext.Current);
        private EBO _indexBuffer;

        private int _indexBufferSize;
        private VBO _vertexBuffer;
        private int _vertexBufferSize;

     
        [MemberNotNull(nameof(fontTexture))]
        public void CreateDeviceResources(IRenderingContext context)
        {
            CreateShader(context);

            RecreateFontDeviceTexture(context);

            _vertexBufferSize = 10000;
            _indexBufferSize = 2000;

            vertexArray.Bind(context);

            _vertexBuffer = new VBO(new BufferSettings(_vertexBufferSize, BufferUsageHint.DynamicDraw))
                .Setup(context)
                .Bind(context);
            context.BufferData(BufferTarget.ArrayBuffer, _vertexBuffer.Settings.BufferSize, nint.Zero, _vertexBuffer.Settings.BufferUsageHint);

            _indexBuffer = new EBO(new BufferSettings(_indexBufferSize, BufferUsageHint.DynamicDraw))
                .Setup(context)
                .Bind(context);
            context.BufferData(BufferTarget.ElementArrayBuffer, _indexBufferSize, nint.Zero, BufferUsageHint.DynamicDraw);

            void SetAttribute(int index, int size, VertexAttribType type, int relativeoffset, bool normalized = false)
            {
                int stride = Unsafe.SizeOf<ImDrawVert>();
                context.VertexArrayVertexBuffer(vertexArray.Handle, index, _vertexBuffer.Handle, nint.Zero, stride)
                    .VertexArrayAttribBinding(vertexArray.Handle, index, index)
                    .VertexArrayAttribFormat(vertexArray.Handle, index, size, type, normalized, relativeoffset)
                    .EnableVertexArrayAttrib(vertexArray.Handle, index);
            }
            SetAttribute(0, 2, VertexAttribType.Float, 0);
            SetAttribute(1, 2, VertexAttribType.Float, 8);
            SetAttribute(2, 4, VertexAttribType.UnsignedByte, 16, true);

            context.BindVAO(0)
                .BindBuffer(BufferTarget.ElementArrayBuffer, 0)
                .BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private void CreateShader(IRenderingContext context)
        {
            _shader = ShaderProgram.GetOrCreate("ImGui",
                    new VertexShader("Resources/Shaders/ImGui/ImGui.vert"),
                    new FragmentShader("Resources/Shaders/ImGui/ImGui.frag"));
            _shader.Setup(context);

            context.GetUniformLocation(_shader.Handle, "projection_matrix", out _shaderProjectionMatrixLocation)
                .GetUniformLocation(_shader.Handle, "in_fontTexture", out _shaderFontTextureLocation);
        }

        [MemberNotNull(nameof(fontTexture))]
        public void RecreateFontDeviceTexture(IRenderingContext context)
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.Fonts.GetTexDataAsRGBA32(out nint pixels, out int width, out int height, out _);

            fontTexture?.Dispose();
            var settings = new TextureSettings()
            {
                TextureTarget = TextureTarget.Texture2D,
                SizedInternalFormat = SizedInternalFormat.Rgba8,
            };
            fontTexture = new Texture(new Vector2i(width, height), settings);
            fontTexture.Setup(context);
            context.TextureSubImage2D(fontTexture.Handle, 0, 0, 0, width, height, PixelFormat.Bgra, PixelType.UnsignedByte, pixels);

            io.Fonts.SetTexID(fontTexture.Handle);
            io.Fonts.ClearTexData();
        }

        private void RenderImDrawData(IRenderingContext context, ImDrawDataPtr draw_data)
        {
            if(0 == draw_data.CmdListsCount)
            {
                return;
            }

            vertexArray.Bind(context);
            _indexBuffer.Bind(context);
            _vertexBuffer.Bind(context);
            context.BindFBOAs(FramebufferTarget.Framebuffer,0);
            context.Viewport(0,0, (int)draw_data.DisplaySize.X, (int)draw_data.DisplaySize.Y);
            for(int i = 0; i < draw_data.CmdListsCount; i++)
            {
                ImDrawListPtr cmd_list = draw_data.CmdLists[i];

                int vertexSize = cmd_list.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>();
                if(vertexSize > _vertexBufferSize)
                {
                    int newSize = (int)Math.Max(_vertexBufferSize * 1.5f, vertexSize);

                    context.BufferData(BufferTarget.ArrayBuffer, newSize, nint.Zero, BufferUsageHint.DynamicDraw);
                    _vertexBufferSize = newSize;
                    _vertexBuffer.Settings.BufferSize = _vertexBufferSize;
                    Debug.WriteLine($"Resized dear imgui vertex buffer to new size {_vertexBufferSize}");
                }

                int indexSize = cmd_list.IdxBuffer.Size * sizeof(ushort);
                if(indexSize > _indexBufferSize)
                {
                    int newSize = (int)Math.Max(_indexBufferSize * 1.5f, indexSize);
                    context.BufferData(BufferTarget.ElementArrayBuffer, newSize, nint.Zero, BufferUsageHint.DynamicDraw); 
                    _indexBufferSize = newSize;
                    _indexBuffer.Settings.BufferSize = _indexBufferSize;

                    Debug.WriteLine($"Resized dear imgui index buffer to new size {_indexBufferSize}");
                }
            }

            // Setup orthographic projection matrix into our constant buffer
            ImGuiIOPtr io = ImGui.GetIO();
            Matrix4 mvp = Matrix4.CreateOrthographicOffCenter(0.0f, io.DisplaySize.X, io.DisplaySize.Y, 0.0f, -1.0f, 1.0f);
            _shader.Bind(context)
                .SetShaderData(context, _shaderProjectionMatrixLocation, mvp)
                .SetShaderData(context, _shaderFontTextureLocation, 0);

            draw_data.ScaleClipRects(io.DisplayFramebufferScale);
            _shader.Bind(context);

            int[] originalViewport = new int[4];

            context.IsEnabled(EnableCap.Blend, out bool blendEnabled)
                .IsEnabled(EnableCap.ScissorTest, out bool scissorTestEnabled)
                .IsEnabled(EnableCap.CullFace, out bool cullFaceEnabled)
                .IsEnabled(EnableCap.DepthTest, out bool depthTestEnabled)
                .GetInteger(GetPName.BlendEquationRgb, out int originalBlendEquation) // Assuming RGB and Alpha blend equations are the same
                .GetInteger(GetPName.BlendSrcRgb, out int originalBlendSrc) // Assuming RGB and Alpha blend funcs are the same
                .GetInteger(GetPName.BlendDstRgb, out int originalBlendDst)
                .GetIntegerv(GetPName.Viewport, originalViewport);

            context
                .Enable(EnableCap.Blend)
                .Enable(EnableCap.ScissorTest)
                .BlendEquation(BlendEquationMode.FuncAdd)
                .BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha)
                .Disable(EnableCap.CullFace)
                .Disable(EnableCap.DepthTest);

            // Render command lists
            for(int n = 0; n < draw_data.CmdListsCount; n++)
            {
                ImDrawListPtr cmd_list = draw_data.CmdLists[n];
                context.NamedBufferSubData(_vertexBuffer.Handle, nint.Zero, cmd_list.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>(), cmd_list.VtxBuffer.Data)
                    .NamedBufferSubData(_indexBuffer.Handle, nint.Zero, cmd_list.IdxBuffer.Size * sizeof(ushort), cmd_list.IdxBuffer.Data);

                for(int cmd_i = 0; cmd_i < cmd_list.CmdBuffer.Size; cmd_i++)
                {
                    ImDrawCmdPtr pcmd = cmd_list.CmdBuffer[cmd_i];
                    if(pcmd.UserCallback != nint.Zero)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        context.BindTextureUnit(0, (int)pcmd.TextureId);

                        // We do _windowHeight - (int)clip.W instead of (int)clip.Y because gl has flipped Y when it comes to these coordinates
                        var clip = pcmd.ClipRect;
                        context.Scissor((int)clip.X, (int)io.DisplaySize.Y - (int)clip.W, (int)(clip.Z - clip.X), (int)(clip.W - clip.Y));

                        if((io.BackendFlags & ImGuiBackendFlags.RendererHasVtxOffset) != 0)
                        {
                            context.DrawElementsBaseVertex(PrimitiveType.Triangles, (int)pcmd.ElemCount, DrawElementsType.UnsignedShort, (nint)(pcmd.IdxOffset * sizeof(ushort)), (int)pcmd.VtxOffset);
                        }
                        else
                        {
                            context.DrawElements(PrimitiveType.Triangles, (int)pcmd.ElemCount, DrawElementsType.UnsignedShort, (int)pcmd.IdxOffset * sizeof(ushort));
                        }
                    }
                }
            }
            context.BindBuffer(BufferTarget.ElementArrayBuffer, 0)
                .BindBuffer(BufferTarget.ArrayBuffer, 0)
                .UseProgram(0)
                .BindTextureUnit(0, 0)
                .BindVAO(0);

            if(blendEnabled)
            {
                context.Enable(EnableCap.Blend);
            }
            else
            {
                context.Disable(EnableCap.Blend);
            }

            if(scissorTestEnabled)
            {
                context.Enable(EnableCap.ScissorTest);
            }
            else
            {
                context.Disable(EnableCap.ScissorTest);
            }

            if(cullFaceEnabled)
            {
                context.Enable(EnableCap.CullFace);
            }
            else
            {
                context.Disable(EnableCap.CullFace);
            }

            if(depthTestEnabled)
            {
                context.Enable(EnableCap.DepthTest);
            }
            else
            {
                context.Disable(EnableCap.DepthTest);
            }

            context.BlendEquation((BlendEquationMode)originalBlendEquation);
            context.BlendFunc((BlendingFactor)originalBlendSrc, (BlendingFactor)originalBlendDst);
            context.Viewport(originalViewport[0], originalViewport[1], originalViewport[2], originalViewport[3]);
        }
    }
}
