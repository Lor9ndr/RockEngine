using OpenTK.Graphics.OpenGL4;

using RockEngine.Assets;

namespace RockEngine.Rendering.Commands
{
    internal readonly struct DrawMeshCommand : ICommand
    {
        private readonly Mesh _mesh;
        public DrawMeshCommand(Mesh mesh)
        {
            _mesh = mesh;
        }

        public void Execute()
        {
            _mesh.VAO.BindIfNotBinded();

            if(_mesh.InstanceCount > 1)
            {
                //TODO: INSTANCE RENDERING 
                //THINK ABOUT MATERIALS AND OTHER COMPONENTS ATTACHED TO THE GameObject
                if(_mesh.HasIndices)
                {
                    _mesh.EBO!.Bind();
                    GL.DrawElementsInstanced(PrimitiveType.Triangles, _mesh.Indices!.Length, DrawElementsType.UnsignedInt, nint.Zero, instancecount: _mesh.InstanceCount);
                }
                else
                {
                    GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, _mesh.Vertices!.Length, _mesh.InstanceCount);
                }
            }
            else
            {
                if(_mesh.HasIndices)
                {
                    _mesh.EBO.BindIfNotBinded();
                    GL.DrawElements(PrimitiveType.Triangles, _mesh.Indices!.Length, DrawElementsType.UnsignedInt, 0);
                }
                else
                {
                    GL.DrawArrays(PrimitiveType.Triangles, 0, _mesh.Vertices!.Length);
                }
            }
        }
    }
}
