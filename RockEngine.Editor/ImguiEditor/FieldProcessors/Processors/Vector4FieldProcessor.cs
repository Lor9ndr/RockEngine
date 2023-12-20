using ImGuiNET;

using OpenTK.Mathematics;

using System.Reflection;

namespace RockEngine.Editor.ImguiEditor.FieldProcessors.Processors
{
    internal sealed class Vector4FieldProcessor : IUIFieldProcessor
    {
        public bool CanProcess(Type fieldType) => fieldType == typeof(Vector4);

        public void Process(ref object obj, ref object value, FieldInfo field, UIAttribute? attribute = null)
        {
            var setter = FieldProcessor.GetOrCreateFieldSetter(field);
            var alias = FieldProcessor.CreateAlias(obj, field, attribute);

            var realValue = (System.Numerics.Vector4)value;
            if(attribute is not null && attribute.IsColor)
            {
                if(ImGui.ColorEdit4(alias, ref realValue))
                {
                    setter(obj, (Vector4)realValue);
                }
            }
            else
            {
                if(ImGui.DragFloat4(alias, ref realValue))
                {
                    setter(obj, (Vector4)realValue);
                }
            }
        }
    }
}
