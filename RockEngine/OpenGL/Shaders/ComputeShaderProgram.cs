using OpenTK.Graphics.OpenGL4;

namespace RockEngine.OpenGL.Shaders
{
    public class ComputeShaderProgram : AShaderProgram
    {
        public ComputeShaderProgram(string name, ComputeShader computeShader)
            : base(name, computeShader)
        {
        }
        public override bool IsBinded()
            => GL.GetInteger(GetPName.CurrentProgram) == Handle;

        /*  public void Dispatch(int numGroupX, int numGroupY, int numGroupZ)
          {
              Use(() =>
              {
                  GL.DispatchCompute(numGroupX, numGroupY, numGroupZ);
              });
          }*/

        /*     public T[] GetDataFromBuffer<T>(SSBO<T> buffer) where T : struct
             {
                 GL.BindBuffer(BufferTarget.ShaderStorageBuffer, buffer.Handle);

                 // Map the buffer to client memory
                 IntPtr bufferData = GL.MapBuffer(BufferTarget.ShaderStorageBuffer, BufferAccess.ReadOnly);

                 // Create a C# array to store the data
                 T[] insideFrustumIndexes = new T[(int)(buffer.Settings.Size/ Marshal.SizeOf<T>())];

                 // Copy the data from the mapped buffer to the C# array
                 unsafe
                 {
                     fixed (T* ptr = insideFrustumIndexes)
                     {
                         System.Buffer.MemoryCopy(bufferData.ToPointer(), ptr, (long)buffer.Settings.Size, (long)buffer.Settings.Size);
                     }
                 }

                 // Unmap the buffer
                 GL.UnmapBuffer(BufferTarget.ShaderStorageBuffer);

                 // Unbind the buffer object
                 GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);

                 return insideFrustumIndexes;
             }*/
    }
}
