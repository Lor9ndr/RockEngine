using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.Rendering.OpenGL;
using RockEngine.Rendering.OpenGL.Buffers;
using RockEngine.Rendering.OpenGL.Settings;
using RockEngine.Rendering.OpenGL.Shaders;
using RockEngine.Rendering.OpenGL.Textures;

namespace RockEngine.Rendering
{
    public class OpenGLRenderingContext : IRenderingContext
    {
        public IRenderingContext AttachShader(int program, int shader)
        {
            GL.AttachShader(program, shader);
            return this;
        }

        public IRenderingContext Bind(VBO glObject)
        {
            return BindBuffer(BufferTarget.ArrayBuffer, glObject.Handle);
        }

        public IRenderingContext Bind(EBO glObject)
        {
            return BindBuffer(BufferTarget.ElementArrayBuffer, glObject.Handle);
        }

        public IRenderingContext Bind(FBO glObject)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, glObject.Handle);
            return this;
        }

        public IRenderingContext Bind(IBO glObject)
        {
            return BindBuffer(BufferTarget.DrawIndirectBuffer, glObject.Handle);
        }

        public IRenderingContext Bind(RBO glObject)
        {
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, glObject.Handle);
            return this;
        }

        public IRenderingContext Bind<T>(UBO<T> glObject) where T : struct
        {
            BindBuffer(BufferTarget.UniformBuffer, glObject.Handle);
            return this;
        }

        public IRenderingContext Bind(VAO glObject)
        {
            GL.BindVertexArray(glObject.Handle);
            return this;
        }

        public IRenderingContext Bind(AShaderProgram shader)
        {
            GL.UseProgram(shader.Handle);
            return this;
        }

        public IRenderingContext Bind(TextureTarget target, int handle)
        {
            GL.BindTexture(target, handle);
            return this;
        }

        public IRenderingContext BindBuffer(BufferTarget target, int buffer)
        {
            GL.BindBuffer(target, buffer);
            return this;
        }

        public IRenderingContext BindBufferRange(BufferRangeTarget target, int index, int buffer, int offset, int size)
        {
            GL.BindBufferRange(target, index, buffer, offset, size);
            return this;
        }

        public IRenderingContext BindFBOAs(FramebufferTarget target, int handle)
        {
            GL.BindFramebuffer(target, handle);
            return this;
        }

        public IRenderingContext BindRenderBuffer(RenderbufferTarget target, int handle)
        {
            GL.BindRenderbuffer(target, handle);
            return this;
        }

        public IRenderingContext BindTextureUnit(int unit, int handle)
        {
            GL.BindTextureUnit(unit, handle);
            return this;
        }

        public IRenderingContext ClientWaitSync(nint handle, ClientWaitSyncFlags flags, long timeout, out WaitSyncStatus status)
        {
            status = GL.ClientWaitSync(handle, flags, timeout);
            return this;
        }

        public IRenderingContext CompileShader(int shader, out int status)
        {
            GL.CompileShader(shader);

            // Check for compilation errors
            GL.GetShader(shader, ShaderParameter.CompileStatus, out status);
            return this;
        }

        public IRenderingContext CreateFenceSync(SyncCondition condition, WaitSyncFlags flags, out nint handle)
        {
            handle = GL.FenceSync(condition, flags);
            return this;
        }

        public IRenderingContext CreateFrameBuffer(out int handle)
        {
            GL.CreateFramebuffers(1, out handle);
            return this;
        }

        public IRenderingContext CreateProgram(out int handle)
        {
            handle = GL.CreateProgram();
            return this;
        }

        public IRenderingContext CreateRenderBuffer(out int handle)
        {
            GL.CreateRenderbuffers(1, out handle);
            return this;
        }

        public IRenderingContext CreateShader(ShaderType type, out int handle)
        {
            handle = GL.CreateShader(type);
            return this;
        }

        public IRenderingContext CreateShaderProgram(ShaderType type, int count, string[] shaderTexts, out int handle)
        {
            handle = GL.CreateShaderProgram(type, count, shaderTexts);
            return this;
        }

        public IRenderingContext CreateTexture(TextureTarget target, out int handle)
        {
            GL.CreateTextures(target, 1, out handle);
            return this;
        }

        public IRenderingContext CreateVertexArray(out int handle)
        {
            GL.CreateVertexArrays(1, out handle);
            return this;
        }

        public IRenderingContext DeleteBuffer(int buffer)
        {
            GL.DeleteBuffer(buffer);
            return this;
        }

        public IRenderingContext DeleteFrameBuffer(int handle)
        {
            GL.DeleteFramebuffer(handle);
            return this;
        }

        public IRenderingContext DeleteProgram(int handle)
        {
            GL.DeleteProgram(handle);
            return this;
        }

        public IRenderingContext DeleteRenderBuffer(int handle)
        {
            GL.DeleteRenderbuffer(handle);
            return this;
        }

        public IRenderingContext DeleteShader(int handle)
        {
            GL.DeleteShader(handle);
            return this;
        }

        public IRenderingContext DeleteTexture(int handle)
        {
            GL.DeleteTexture(handle);
            return this;
        }

        public IRenderingContext DeleteVertexArray(int handle)
        {
            GL.DeleteVertexArray(handle);
            return this;
        }

        public IRenderingContext DetachShader(int handle, int shader)
        {
            GL.DetachShader(handle, shader);
            return this;
        }

        public IRenderingContext DrawBuffer(DrawBufferMode mode)
        {
            GL.DrawBuffer(mode);
            return this;
        }

        public IRenderingContext EnableVertexArrayAttrib(int handle, int attribute)
        {
            GL.EnableVertexArrayAttrib(handle, attribute);
            return this;
        }

        public IRenderingContext GetActiveUniform(int handle, int index, int bufSize, out int length, out ActiveUniformType type, out string name)
        {
            GL.GetActiveUniform(handle, index, bufSize, out length, out int _, out type, out name);
            return this;
        }

        public IRenderingContext GetInteger(GetPName name, out int value)
        {
            GL.GetInteger(name, out value);
            return this;
        }

        public IRenderingContext GetObjectLabel(ObjectLabelIdentifier objectLabelIdentifier, int obj, int bufSize, out int length, out string label)
        {
            GL.GetObjectLabel(objectLabelIdentifier, obj, bufSize, out length, out label);
            return this;
        }

        public IRenderingContext GetProgram(int handle, GetProgramParameterName pname, out int param)
        {
            GL.GetProgram(handle, pname, out param);
            return this;
        }

        public IRenderingContext GetProgramInfoLog(int handle, out string infoLog)
        {
            GL.GetProgramInfoLog(handle, out infoLog);
            return this;
        }

        public IRenderingContext GetShaderInfoLog(int shader, out string infoLog)
        {
            GL.GetShaderInfoLog(shader, out infoLog);
            return this;
        }

        public IRenderingContext GetUniformLocation(int handle, string name, out int location)
        {
            location = GL.GetUniformLocation(handle, name);
            return this;
        }

        public IRenderingContext LinkProgram(int handle, out int status)
        {
            GL.LinkProgram(handle);
            GetProgram(handle, GetProgramParameterName.LinkStatus, out status);
            return this;
        }

        public IRenderingContext MapBuffer(int handle, BufferAccess access, out nint buffer)
        {
            buffer = GL.MapNamedBuffer(handle, access);
            return this;
        }

        public IRenderingContext MapBufferRange(ASetuppableGLObject<BufferSettings> buffer, int offset, int size, BufferAccessMask bufferAccessMask, out nint mappedBuffer)
        {
            mappedBuffer = GL.MapNamedBufferRange(buffer.Handle, offset, size, bufferAccessMask);
            return this;
        }

        public IRenderingContext NamedBufferData<T>(int buffer, int size, ref T data, BufferUsageHint hint) where T : struct
        {
            GL.NamedBufferData(buffer, size, ref data, hint);
            return this;
        }

        public IRenderingContext NamedBufferData(int buffer, int size, nint data, BufferUsageHint hint)
        {
            GL.NamedBufferData(buffer, size, data, hint);
            return this;
        }

        public IRenderingContext NamedBufferStorage(int handle, int size, nint data, BufferStorageFlags flags)
        {
            GL.NamedBufferStorage(handle, size, data, flags);
            return this;
        }

        public IRenderingContext NamedBufferStorage<T>(int handle, int size, T[] data, BufferStorageFlags flags) where T : struct
        {
            GL.NamedBufferStorage(handle, size, data, flags);
            return this;
        }

        public IRenderingContext NamedBufferSubData(int buffer, int offset, int size, nint data)
        {
            GL.NamedBufferSubData(buffer, offset, size, data);
            return this;
        }

        public IRenderingContext NamedBufferSubData<T>(int buffer, int offset, int size, ref T data) where T : struct
        {
            GL.NamedBufferSubData(buffer, offset, size, ref data);
            return this;
        }

        public IRenderingContext NamedFramebufferTexture(int framebuffer, FramebufferAttachment attachment, int texture, int level)
        {
            GL.NamedFramebufferTexture(framebuffer, attachment, texture, level);
            return this;
        }

        public IRenderingContext NamedRenderbufferStorage(int renderbuffer, RenderbufferStorage internalformat, int width, int height)
        {
            GL.NamedRenderbufferStorage(renderbuffer, internalformat, width, height);
            return this;
        }

        public IRenderingContext NamedRenderbufferStorageMultisample(int renderbuffer, int samples, RenderbufferStorage internalformat, int width, int height)
        {
            GL.NamedRenderbufferStorageMultisample(renderbuffer, samples, internalformat, width, height);
            return this;
        }

        public IRenderingContext ReadBuffer(ReadBufferMode mode)
        {
            GL.ReadBuffer(mode);
            return this;
        }

        public IRenderingContext ReadPixel<T>(int x, int y, int width, int height, PixelFormat format, PixelType type, ref T info) where T : struct
        {
            GL.ReadPixels(x, y, width, height, format, type, ref info);
            return this;
        }

        public IRenderingContext SetShaderData(int location, Vector4 data)
        {
            GL.Uniform4(location, data);
            return this;
        }

        public IRenderingContext SetShaderData(int location, Color4 data)
        {
            GL.Uniform4(location, data);
            return this;
        }

        public IRenderingContext SetShaderData(int location, float data)
        {
            GL.Uniform1(location, data);
            return this;
        }

        public IRenderingContext SetShaderData(int location, float x, float y, float z)
        {
            GL.Uniform3(location, new Vector3(x, y, z));
            return this;
        }

        public IRenderingContext SetShaderData(int location, int data)
        {
            GL.Uniform1(location, data);
            return this;
        }

        public IRenderingContext SetShaderData(int location, Matrix4 data)
        {
            GL.UniformMatrix4(location, false, ref data);
            return this;
        }

        public IRenderingContext SetShaderData(int location, Vector2 data)
        {
            GL.Uniform2(location, data);
            return this;
        }

        public IRenderingContext SetShaderData(int location, Vector3 data)
        {
            GL.Uniform3(location, data);
            return this;
        }

        public IRenderingContext SetTextureParameters(Texture texture)
        {
            if(texture.Settings.TextureParameters is not null)
            {
                foreach(var param in texture.Settings.TextureParameters)
                {
                    TextureParameter(texture.Handle, param.Key, param.Value);
                }
            }
            return this;
        }

        public IRenderingContext TextureParameter(int handle, TextureParameterName paramName, int value)
        {
            GL.TextureParameter(handle, paramName, value);
            return this;
        }

        public IRenderingContext TextureStorage2D(int handle, int levels, SizedInternalFormat sizedInternalFormat, Vector2i size)
        {
            GL.TextureStorage2D(handle, levels, sizedInternalFormat, size.X, size.Y);
            return this;
        }

        public IRenderingContext TextureStorage2DMultisample(int handle, int samples, SizedInternalFormat sizedInternalFormat, Vector2i size, bool fixedSampleLocations = true)
        {
            GL.TextureStorage2DMultisample(handle, samples, sizedInternalFormat, size.X, size.Y, fixedSampleLocations);
            return this;
        }

        public IRenderingContext UnmapBuffer(int handle)
        {
            GL.UnmapNamedBuffer(handle);
            return this;
        }

        public IRenderingContext UseProgram(int handle)
        {
            GL.UseProgram(handle);
            return this;
        }

        public IRenderingContext VertexAttribDivisor(int binding, int divisor)
        {
            GL.VertexAttribDivisor(binding, divisor);
            return this;
        }

        public IRenderingContext VertexAttribPointer(int index, int size, VertexAttribPointerType type, bool normalized, int stride, int offset)
        {
            GL.VertexAttribPointer(index, size, type, normalized, stride, offset);
            return this;
        }

        public IRenderingContext ObjectLabel(ObjectLabelIdentifier identifier, int handle, int length, string name)
        {
            GL.ObjectLabel(identifier, handle, length, name);
            return this;
        }

        public IRenderingContext ShaderSource(int handle, string shaderSource)
        {
            GL.ShaderSource(handle, shaderSource);
            return this;
        }

        public IRenderingContext ReadPixel<T>(int x, int y, int width, int height, PixelFormat format, PixelType type, T[] info) where T : struct
        {
            GL.ReadPixels(x, y, width, height, format, type, info);
            return this;
        }

        public IRenderingContext BindBufferRange(BufferRangeTarget target, int index, int buffer, nint offset, int size)
        {
            GL.BindBufferRange(target, index, buffer, offset, size);
            return this;
        }

        public IRenderingContext NamedBufferData<T>(int buffer, int size, T[] data, BufferUsageHint hint) where T : struct
        {
            GL.NamedBufferData(buffer, size, data, hint);
            return this;
        }

        public IRenderingContext CheckNamedFramebufferStatus(int handle, FramebufferTarget framebufferTarget, out FramebufferStatus status)
        {
            status = GL.CheckNamedFramebufferStatus(handle, framebufferTarget);
            return this;
        }

        public IRenderingContext CreateBuffer(out int handle)
        {
            GL.CreateBuffers(1, out handle);
            return this;
        }

        public IRenderingContext GetUniformBlockIndex(int handle, string? bufferName, out int index)
        {
            index = GL.GetUniformBlockIndex(handle, bufferName);
            return this;
        }

        public IRenderingContext UniformBlockBinding(int handle, int index, int bindingPoint)
        {
            GL.UniformBlockBinding(handle, index, bindingPoint);
            return this;
        }

        public IRenderingContext GetActiveUniformBlock(int handle, int index, ActiveUniformBlockParameter param, out int value)
        {
            GL.GetActiveUniformBlock(handle, index, param, out value);
            return this;
        }

        public IRenderingContext DrawElements(PrimitiveType primitiveType, int count, DrawElementsType elementsType, int indices)
        {
            GL.DrawElements(primitiveType, count, elementsType, indices);
            return this;
        }

        public IRenderingContext DrawArrays(PrimitiveType primitiveType, int first, int count)
        {
            GL.DrawArrays(primitiveType, first, count);
            return this;
        }

        public IRenderingContext DrawArraysInstanced(PrimitiveType primitiveType, int first, int count, int instanceCount)
        {
            GL.DrawArraysInstanced(primitiveType, first, count, instanceCount);
            return this;
        }

        public IRenderingContext DrawElementsInstanced(PrimitiveType primitiveType, int count, DrawElementsType type, nint indices, int instanceCount)
        {
            GL.DrawElementsInstanced(primitiveType, count, type, indices, instanceCount);
            return this;
        }

        public IRenderingContext GetUniform(int handle, int location, float[] value)
        {
            GL.GetUniform(handle, location, value);
            return this;
        }

        public IRenderingContext BindVAO(int handle)
        {
            GL.BindVertexArray(handle);
            return this;
        }

        public IRenderingContext VertexAttribPointer(int index, int size, VertexAttribPointerType type, bool normalized, int stride, nint offset)
        {
            GL.VertexAttribPointer(index, size, type, normalized, stride, offset);
            return this;
        }

        public IRenderingContext VertexArrayAttribDivisor(int handle, int binding, int divisor)
        {
            GL.VertexArrayBindingDivisor(handle, binding, divisor);
            return this;
        }

        public IRenderingContext DeleteSync(nint syncObj)
        {
            GL.DeleteSync(syncObj);
            return this;
        }

        public IRenderingContext VertexArrayVertexBuffer(int vao, int bindingIndex, int buffer, nint offset, int stride)
        {
            GL.VertexArrayVertexBuffer(vao, bindingIndex, buffer, offset, stride);
            return this;
        }

        public IRenderingContext LineWidth(float lineWidth)
        {
            GL.LineWidth(lineWidth);
            return this;
        }

        public IRenderingContext Disable(EnableCap cap)
        {
            GL.Disable(cap);
            return this;
        }

        public IRenderingContext NamedBufferSubData<T>(int handle, nint offset, int size, T[] data) where T : struct
        {
            GL.NamedBufferSubData(handle, offset, size, data);
            return this;
        }

        public IRenderingContext NamedBufferSubData(int handle, nint offset, int size, nint data)
        {
            GL.NamedBufferSubData(handle, offset, size, data);
            return this;
        }

        public IRenderingContext NamedBufferSubData<T>(int handle, nint offset, int size, ref T data) where T : struct
        {
            GL.NamedBufferSubData(handle, offset, size, ref data);
            return this;
        }

        public IRenderingContext NamedFramebufferRenderbuffer(int fbo, FramebufferAttachment renderBufferAttachment, RenderbufferTarget renderbufferTarget, int rbo)
        {
            GL.NamedFramebufferRenderbuffer(fbo, renderBufferAttachment, renderbufferTarget, rbo);
            return this;
        }

        public IRenderingContext Viewport(int v1, int v2, int x, int y)
        {
            GL.Viewport(v1, v2, x, y);
            return this;
        }

        public IRenderingContext Clear(ClearBufferMask clearBufferMask)
        {
            GL.Clear(clearBufferMask);
            return this;
        }

        public IRenderingContext ClearColor(int v1, int v2, int v3, int v4)
        {
            GL.ClearColor(v1, v2, v3, v4);
            return this;
        }

        public IRenderingContext CreateTextures(TextureTarget textureTarget, out int handle)
        {
            GL.CreateTextures(textureTarget, 1, out handle);
            return this;
        }

        public IRenderingContext StencilOp(StencilOp op, StencilOp keep2, StencilOp replace)
        {
            GL.StencilOp(op, keep2, replace);
            return this;
        }

        public IRenderingContext Enable(EnableCap cap)
        {
            GL.Enable(cap);
            return this;
        }

        public IRenderingContext StencilFunc(StencilFunction func, int v1, int v2)
        {
            GL.StencilFunc(func, v1,v2);
            return this;
        }

        public IRenderingContext StencilMask(int v)
        {
            GL.StencilMask(v);
            return this;
        }

        public IRenderingContext ClearColor(Color4 backGroundColor)
        {
            GL.ClearColor(backGroundColor);
            return this;
        }

        public IRenderingContext PolygonMode(MaterialFace face, PolygonMode mode)
        {
            GL.PolygonMode(face, mode);
            return this;
        }

        public IRenderingContext TextureSubImage2D(int handle, int level, int yOffset, int xOffset, int width, int height, PixelFormat format, PixelType type, nint pixels)
        {
            GL.TextureSubImage2D(handle, level, xOffset, yOffset, width, height, format, type, pixels);
            return this;
        }

        public IRenderingContext VertexArrayAttribFormat(int handle, int index, int size, VertexAttribType type, bool normalized, int relativeoffset)
        {
            GL.VertexArrayAttribFormat(handle, index, size, type, normalized, relativeoffset);
            return this;
        }

        public IRenderingContext VertexArrayAttribBinding(int handle, int index1, int index2)
        {
            GL.VertexArrayAttribBinding(handle, index1, index2);
            return this;
        }

        public IRenderingContext BlendEquation(BlendEquationMode mode)
        {
            GL.BlendEquation(mode);
            return this;
        }

        public IRenderingContext BlendFunc(BlendingFactor factor, BlendingFactor factor2)
        {
            GL.BlendFunc(factor, factor2);
            return this;
        }

        public IRenderingContext Scissor(int x, int v1, int v2, int v3)
        {
            GL.Scissor(x, v1, v2, v3);
            return this;
        }

        public IRenderingContext DrawElementsBaseVertex(PrimitiveType primType, int elemCount, DrawElementsType type, nint value, int offset)
        {
            GL.DrawElementsBaseVertex(primType, elemCount, type, value, offset);
            return this;
        }

        public IRenderingContext BufferData(BufferTarget target, int size, nint data, BufferUsageHint hint)
        {
            GL.BufferData(target, size, data, hint);
            return this;
        }

        public IRenderingContext BufferSubData(BufferTarget target, nint offset, int size, nint data)
        {
            GL.BufferSubData(target, offset, size, data);
            return this;
        }

        public IRenderingContext IsEnabled(EnableCap param, out bool value)
        {
            value = GL.IsEnabled(param);
            return this;
        }

        public IRenderingContext GetIntegerv(GetPName param, int[] values)
        {
            GL.GetInteger(param, values);
            return this;
        }

        public IRenderingContext GenerateTextureMipmap(int handle)
        {
            GL.GenerateTextureMipmap(handle);
            return this;
        }

        public IRenderingContext NamedBufferSubData<T>(int buffer, int offset, int size, T[] data) where T : struct
        {
            GL.NamedBufferSubData(buffer, offset, size, data);
            return this;
        }

        public IRenderingContext InvalidateBufferSubData(int handle, int offset, int size)
        {
            GL.InvalidateBufferSubData(handle, offset, size);
            return this;
        }

        public IRenderingContext CopyNamedBufferSubData(int handle, int newBufferHandle, int readOffset, int writeOffset, int sizeToCopy)
        {
            GL.CopyNamedBufferSubData(handle, newBufferHandle, readOffset, writeOffset, sizeToCopy);
            return this;
        }
    }
}
