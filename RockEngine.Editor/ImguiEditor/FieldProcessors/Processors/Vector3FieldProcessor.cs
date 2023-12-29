using ImGuiNET;

using OpenTK.Mathematics;

using RockEngine.Common.Editor;

using System.Reflection;

namespace RockEngine.Editor.ImguiEditor.FieldProcessors.Processors
{
    internal sealed class Vector3FieldProcessor : IUIFieldProcessor
    {
        public bool CanProcess(Type fieldType)
        {
            return fieldType == typeof(Vector3);
        }

      
        public void Process(ref object value, FieldInfo field, UIAttribute attribute)
        {
            var alias = FieldProcessor.CreateAlias(value, field, attribute);
            var realValue = (Vector3)value;
            var guilValue = (System.Numerics.Vector3)realValue;
            if(attribute is not null && attribute.IsColor)
            {
                if(ImGui.ColorEdit3(alias, ref guilValue))
                {
                    value = (Vector3)guilValue;
                }
            }
            else
            {
                if(ImGui.DragFloat3(alias, ref guilValue, attribute.Speed, attribute.Min, attribute.Max))
                {
                    value = (Vector3)guilValue;
                }
            }
        }
    }
}
