using ImGuiNET;

using RockEngine.Common.Editor;

using System.Reflection;

namespace RockEngine.Editor.ImguiEditor.FieldProcessors.Processors
{
    internal sealed class FloatFieldProcessor : IUIFieldProcessor
    {
        public bool CanProcess(Type fieldType)
        {
            return fieldType == typeof(float);
        }

        public void Process(ref object value, FieldInfo field, UIAttribute attribute)
        {
            string alias = FieldProcessor.CreateAlias(value, field, attribute);
            var number = (float)value;

            if(ImGui.DragFloat(alias, ref number, attribute.Speed, attribute.Min, attribute.Max))
            {
                value = number;
            }
        }
    }
}
