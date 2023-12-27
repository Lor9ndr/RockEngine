using System.Runtime.CompilerServices;

namespace RockEngine.ShaderGenerator
{
    internal class LayoutIn<T> :Variable where T : struct
    {
        public int Location { get;set;}

        public LayoutIn(int location)
        {
            Location = location;
        }
        public override string GetString([CallerMemberName] string fieldName = "")
        {
            return $"layout (location = {Location}) in {this.ConvertToGLSLType(typeof(T))} {fieldName};";
        }
    }

    internal class LayoutOut<T> : Variable where T : struct
    {
        public int Location { get; set; }

        public LayoutOut(int location)
        {
            Location = location;
        }

        public override string GetString([CallerMemberName]string fieldName = "")
        {
            return $"layout (location = {Location}) out {this.ConvertToGLSLType(typeof(T))} {fieldName};";
        }
    }

    internal class Out<T> : Variable where T : struct
    {
        public override string GetString([CallerMemberName] string fieldName = "")
        {
            return $"out {this.ConvertToGLSLType(typeof(T))} {fieldName};";
        }
    }
}
