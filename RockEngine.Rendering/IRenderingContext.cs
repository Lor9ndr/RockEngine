#define OpenGL

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.Rendering.OpenGL;
using RockEngine.Rendering.OpenGL.Buffers;
using RockEngine.Rendering.OpenGL.Settings;
using RockEngine.Rendering.OpenGL.Shaders;
using RockEngine.Rendering.OpenGL.Textures;
namespace RockEngine.Rendering
{
    public interface IRenderingContext
    {
#if OpenGL
        public static readonly IRenderingContext Current = new OpenGLRenderingContext();
#endif

        public static IRenderingContext Render(Action<IRenderingContext> context)
        {
            OpenGLDispatcher.RenderEnqueue(() => { context(Current); });
            return Current;
        }
        public static IRenderingContext Update(Action<IRenderingContext> context)
        {
            OpenGLDispatcher.UpdateEnqueue(() => { context(Current); });
            return Current;
        }

        #region Common
        public IRenderingContext ObjectLabel(ObjectLabelIdentifier identifier, int handle, int length, string name);

        public IRenderingContext GetObjectLabel(ObjectLabelIdentifier objectLabelIdentifier, int obj, int bufSize, out int length, out string label);
        public IRenderingContext GetInteger(GetPName name, out int value);
        #endregion

        #region Buffers
        public IRenderingContext Bind(VBO glObject);
        public IRenderingContext Bind(EBO glObject);
        public IRenderingContext Bind(FBO glObject);
        public IRenderingContext Bind(IBO glObject);
        public IRenderingContext Bind(RBO glObject);
        public IRenderingContext Bind<T>(UBO<T> glObject) where T : struct;
        public IRenderingContext Bind(VAO glObject);
        public IRenderingContext BindVAO(int handle);

        public IRenderingContext Bind(AShaderProgram shader);
        public IRenderingContext CreateBuffer(out int handle);

        #endregion

        #region FBO

        public IRenderingContext CreateFrameBuffer(out int handle);
        public IRenderingContext ReadBuffer(ReadBufferMode mode);
        public IRenderingContext DrawBuffer(DrawBufferMode mode);
        public IRenderingContext BindFBOAs(FramebufferTarget target, int handle);
        public IRenderingContext NamedFramebufferTexture(int framebuffer, FramebufferAttachment attachment, int texture, int level);
        public IRenderingContext ReadPixel<T>(int x, int y, int width, int height, PixelFormat format, PixelType type, ref T info) where T : struct;
        public IRenderingContext ReadPixel<T>(int x, int y, int width, int height, PixelFormat format, PixelType type, T[] info) where T : struct;
        public IRenderingContext DeleteFrameBuffer(int handle);
        public IRenderingContext CheckNamedFramebufferStatus(int handle, FramebufferTarget framebufferTarget, out FramebufferStatus status);

        #endregion

        #region RBO

        public IRenderingContext CreateRenderBuffer(out int handle);
        public IRenderingContext BindRenderBuffer(RenderbufferTarget target, int handle);
        public IRenderingContext DeleteRenderBuffer(int handle);

        public IRenderingContext NamedRenderbufferStorage(int renderbuffer, RenderbufferStorage internalformat, int width, int height);
        public IRenderingContext NamedRenderbufferStorageMultisample(int renderbuffer, int samples, RenderbufferStorage internalformat, int width, int height);

        #endregion

        #region VAO
        public IRenderingContext DeleteVertexArray(int handle);
        public IRenderingContext CreateVertexArray(out int handle);
        public IRenderingContext EnableVertexArrayAttrib(int handle, int attribute);

        public IRenderingContext VertexAttribPointer(int index, int size, VertexAttribPointerType type, bool normalized, int stride, int offset);
        public IRenderingContext VertexAttribPointer(int index, int size, VertexAttribPointerType type, bool normalized, int stride, nint offset);

        public IRenderingContext VertexAttribDivisor(int binding, int divisor);

        #endregion

        #region BuffersCommon

        public IRenderingContext BindBufferRange(BufferRangeTarget target, int index, int buffer, int offset, int size);
        public IRenderingContext BindBufferRange(BufferRangeTarget target, int index, int buffer, nint offset, int size);
        public IRenderingContext NamedBufferSubData(int buffer, int offset, int size, nint data);
        public IRenderingContext NamedBufferSubData<T>(int buffer, int offset, int size, ref T data) where T : struct;
        public IRenderingContext NamedBufferData<T>(int buffer, int size, ref T data, BufferUsageHint hint) where T : struct;
        public IRenderingContext NamedBufferData<T>(int buffer, int size, T[] data, BufferUsageHint hint) where T : struct;
        public IRenderingContext NamedBufferData(int buffer, int size, nint data, BufferUsageHint hint);
        public IRenderingContext BindBuffer(BufferTarget target, int buffer);
        public IRenderingContext DeleteBuffer(int buffer);
        public IRenderingContext MapBuffer(int handle, BufferAccess access, out nint buffer);
        public IRenderingContext MapBufferRange(ASetuppableGLObject<BufferSettings> buffer, int offset, int size, BufferAccessMask bufferAccessMask, out nint mappedBuffer);

        public IRenderingContext UnmapBuffer(int handle);
        public IRenderingContext NamedBufferStorage(int handle, int size, nint data, BufferStorageFlags flags);
        public IRenderingContext NamedBufferStorage<T>(int handle, int size, T[] data, BufferStorageFlags flags) where T : struct;
        #endregion

        #region Shaders
        public IRenderingContext CreateShader(ShaderType type, out int handle);
        public IRenderingContext AttachShader(int program, int shader);
        public IRenderingContext CompileShader(int shader, out int status);
        public IRenderingContext DetachShader(int handle, int shader);
        public IRenderingContext DeleteShader(int handle);

        public IRenderingContext CreateProgram(out int handle);
        public IRenderingContext CreateShaderProgram(ShaderType type, int count, string[] shaderTexts, out int handle);
        public IRenderingContext UseProgram(int handle);
        public IRenderingContext DeleteProgram(int handle);
        public IRenderingContext LinkProgram(int handle, out int status);

        public IRenderingContext GetShaderInfoLog(int shader, out string infoLog);
        public IRenderingContext GetProgramInfoLog(int handle, out string infoLog);
        public IRenderingContext GetProgram(int handle, GetProgramParameterName pname, out int param);
        public IRenderingContext GetActiveUniform(int handle, int index, int bufSize, out int length, out ActiveUniformType type, out string name);
        public IRenderingContext GetUniformLocation(int handle, string name, out int location);
        public IRenderingContext ShaderSource(int handle, string shaderSource);

        public IRenderingContext GetUniformBlockIndex(int handle, string? bufferName, out int index);
        public IRenderingContext UniformBlockBinding(int handle, int index, int bindingPoint);
        public IRenderingContext GetActiveUniformBlock(int handle, int index, ActiveUniformBlockParameter param, out int value);

        public IRenderingContext SetShaderData(int location, Color4 data);

        /// <summary>
        /// Отправка в шейдер плавающее число
        /// </summary>
        /// <param name="location">Расположение переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public IRenderingContext SetShaderData(int location, float data);


        /// <summary>
        /// Отправка в шейдер 3D вектор
        /// </summary>
        /// <param name="name">Название переменной в шейдере</param>
        /// <param name="x">координата X</param>
        /// <param name="y">координата Y</param>
        /// <param name="z">координата Z</param>
        public IRenderingContext SetShaderData(int location, float x, float y, float z);

        /// <summary>
        /// Отправка в шейдер целочисленную переменную 
        /// </summary>
        /// <param name="location">Расположение переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public IRenderingContext SetShaderData(int location, int data);

        /// <summary>
        /// Отправка в шейдер матрицу 4x4
        /// </summary>
        /// <param name="location">Расположение переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public IRenderingContext SetShaderData(int location, Matrix4 data);

        /// <summary>
        /// Отправка в шейдер 2D вектор
        /// </summary>
        /// <param name="location">Название переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public IRenderingContext SetShaderData(int location, Vector2 data);


        /// <summary>
        /// Отправка в шейдер 3D вектор
        /// </summary>
        /// <param name="location">Расположение переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public IRenderingContext SetShaderData(int location, Vector3 data);

        public IRenderingContext SetShaderData(int location, Vector4 data);

        #endregion

        #region Texture
        public IRenderingContext BindTextureUnit(int unit, int handle);

        public IRenderingContext CreateTexture(TextureTarget target, out int handle);

        public IRenderingContext TextureStorage2DMultisample(int handle, int samples, SizedInternalFormat sizedInternalFormat, Vector2i size, bool fixedSampleLocations = true);

        public IRenderingContext TextureStorage2D(int handle, int levels, SizedInternalFormat sizedInternalFormat, Vector2i size);
        public IRenderingContext TextureParameter(int handle, TextureParameterName paramName, int value);
        public IRenderingContext SetTextureParameters(Texture texture);
        public IRenderingContext Bind(TextureTarget target, int handle);

        public IRenderingContext DeleteTexture(int handle);

        #endregion

        #region SyncObject

        public IRenderingContext CreateFenceSync(SyncCondition condition, WaitSyncFlags flags, out nint handle);
        public IRenderingContext ClientWaitSync(nint handle, ClientWaitSyncFlags flags, long timeout, out WaitSyncStatus status);
        public IRenderingContext DrawElements(PrimitiveType primitiveType, int count, DrawElementsType elementsType, int indices);
        public IRenderingContext DrawArrays(PrimitiveType primitiveType, int first, int count);
        public IRenderingContext DrawArraysInstanced(PrimitiveType primitiveType, int first, int count, int instanceCount);
        public IRenderingContext DrawElementsInstanced(PrimitiveType primitiveType, int count, DrawElementsType type, nint indices, int instanceCount);
        public IRenderingContext GetUniform(int handle, int location, float[] value);
        public IRenderingContext VertexArrayAttribDivisor(int handle, int binding, int divisor);
        public IRenderingContext DeleteSync(nint syncObj);
        public IRenderingContext VertexArrayVertexBuffer(int vao, int bindingIndex, int buffer, nint offset, int stride);
        public IRenderingContext LineWidth(float lineWidth);
        public IRenderingContext Disable(EnableCap cap);
        public IRenderingContext NamedBufferSubData<T>(int handle, nint offset, int size, T[] data) where T : struct;
        public IRenderingContext NamedBufferSubData(int handle, nint offset, int size, nint data);
        public IRenderingContext NamedBufferSubData<T>(int handle, nint offset, int size, ref T data) where T : struct;
        public IRenderingContext NamedFramebufferRenderbuffer(int fbo, FramebufferAttachment renderBufferAttachment, RenderbufferTarget renderbufferTarget, int rbo);
        public IRenderingContext Viewport(int v1, int v2, int x, int y);
        public IRenderingContext Clear(ClearBufferMask clearBufferMask);
        public IRenderingContext ClearColor(int v1, int v2, int v3, int v4);
        public IRenderingContext CreateTextures(TextureTarget textureTarget, out int handle);
        public IRenderingContext StencilOp(StencilOp op, StencilOp keep2, StencilOp replace);
        public IRenderingContext Enable(EnableCap cap);
        public IRenderingContext StencilFunc(StencilFunction func, int v1, int v2);
        public IRenderingContext StencilMask(int v);
        public IRenderingContext ClearColor(Color4 backGroundColor);
        public IRenderingContext PolygonMode(MaterialFace face, PolygonMode mode);
        public IRenderingContext TextureSubImage2D(int handle, int level, int yOffset, int xOffset, int width, int height, PixelFormat format, PixelType type, nint pixels);
        public IRenderingContext VertexArrayAttribFormat(int handle, int index, int size, VertexAttribType type, bool normalized, int relativeoffset);
        public IRenderingContext VertexArrayAttribBinding(int handle, int index1, int index2);
        public IRenderingContext BlendEquation(BlendEquationMode mode);
        public IRenderingContext BlendFunc(BlendingFactor factor, BlendingFactor factor2);
        public IRenderingContext Scissor(int x, int v1, int v2, int v3);
        public IRenderingContext DrawElementsBaseVertex(PrimitiveType primType, int elemCount, DrawElementsType type, nint value, int offset);
        [Obsolete("Use DSA instead, like NamedBufferData")]
        public IRenderingContext BufferData(BufferTarget target, int size, nint data, BufferUsageHint hint);
        [Obsolete("Use DSA instead, like NamedBufferSubData")]
        public IRenderingContext BufferSubData(BufferTarget target, nint offset, int size, nint data);
        public IRenderingContext IsEnabled(EnableCap param, out bool value);
        public IRenderingContext GetIntegerv(GetPName param, int[] values);

        #endregion
    }
}
