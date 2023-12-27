using System.Numerics;

namespace RockEngine.ShaderGenerator
{
    internal class TestShader : BaseVertexShader
    {
        public LayoutIn<Vector3> aPos { get;set;} = new LayoutIn<Vector3>(0);
        public LayoutIn<Vector3> aNormal { get; set; } = new LayoutIn<Vector3>(1);
        public LayoutIn<Vector3> aTexCoords { get; set; } = new LayoutIn<Vector3>(2);

        public Uniform<Matrix4x4> Model { get; set; } = new Uniform<Matrix4x4>();

        public Uniform<Matrix4x4> View { get; set; } = new Uniform<Matrix4x4>();
        public Uniform<Matrix4x4> Projection { get; set; } = new Uniform<Matrix4x4>();
        public Uniform<Vector3> ViewPos { get; set; } = new Uniform<Vector3>();

        public Out<Vector3> FragPos { get; set; } = new Out<Vector3>();

        [Method]
        public void Main()
        {
            FragPos.Value = new Vector3(Model.Value * new Vector4(aPos.Value, 1.0f));
            gl_position = Projection.Value * View.Value * Model.Value * new Vector4(aPos.Value, 1.0f);
        }
    }
}
