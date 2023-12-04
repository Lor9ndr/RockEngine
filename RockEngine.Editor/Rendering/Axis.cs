
using OpenTK.Mathematics;

namespace RockEngine.Editor.Rendering
{
    internal sealed class Axis
    {
        public readonly string Name;
        public readonly Vector3 Color;
        public int Handle;

        public Axis(string name, Vector3 color, int handle)
        {
            Name = name;
            Color = color;
            Handle = handle;
        }
    }
}
