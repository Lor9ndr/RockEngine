using ImGuiNET;

using OpenTK.Mathematics;

using System.Reflection;

namespace RockEngine.Editor.ImguiEditor.FieldProcessors.Processors
{
    internal class QuaternionFieldProcessor : IUIFieldProcessor
    {
        public bool CanProcess(Type fieldType)
        {
            return fieldType == typeof(Quaternion);
        }

        public void Process(ref object value, FieldInfo field, UIAttribute attribute)
        {
            var alias = FieldProcessor.CreateAlias(value, field, attribute);
            Vector4 realValue = new Vector4();
            if(value is Quaternion q)
            {
                realValue = new Vector4(q.X, q.Y, q.Z, q.W);
            }

            var guilValue = new System.Numerics.Vector4(realValue.X, realValue.Y,realValue.Z, realValue.W);
            if(ImGui.DragFloat4(alias, ref guilValue, attribute.Speed, attribute.Min, attribute.Max))
            {
                value = new Quaternion(guilValue.X, guilValue.Y, guilValue.Z, guilValue.W);
            }
        }
    }
}
