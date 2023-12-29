using ImGuiNET;

using OpenTK.Mathematics;

using RockEngine.Common.Editor;

using System.Reflection;

namespace RockEngine.Editor.ImguiEditor.FieldProcessors.Processors
{
    internal sealed class Vector4FieldProcessor : IUIFieldProcessor
    {
        public bool CanProcess(Type fieldType) => fieldType == typeof(Vector4);

        public void Process(ref object value, FieldInfo field, UIAttribute attribute)
        {
            var alias = FieldProcessor.CreateAlias(value, field, attribute);

            var realValue = (System.Numerics.Vector4)value;
            if(attribute is not null && attribute.IsColor)
            {
                if(ImGui.ColorEdit4(alias, ref realValue))
                {
                    value = (Vector4)realValue;
                }
            }
            else
            {
                if(ImGui.DragFloat4(alias, ref realValue))
                {
                    value = (Vector4)realValue;
                }
            }
        }
    }
}
