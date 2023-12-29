using ImGuiNET;

using RockEngine.Common.Editor;

using System.Reflection;

namespace RockEngine.Editor.ImguiEditor.FieldProcessors.Processors
{
    internal sealed class IntFieldProcessor : IUIFieldProcessor
    {
        public bool CanProcess(Type fieldType)
        {
            return fieldType == typeof(int);
        }

        public void Process(ref object value, FieldInfo field, UIAttribute attribute)
        {
            string alias = FieldProcessor.CreateAlias(value, field, attribute);
            var number = (int)value;
            if(ImGui.DragInt(alias, ref number, attribute.Speed, (int)attribute.Min, (int)attribute.Max))
            {
                value = number;
            }
        }
    }
}
