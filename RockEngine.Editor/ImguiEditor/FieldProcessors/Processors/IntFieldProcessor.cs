using ImGuiNET;

using System.Reflection;

namespace RockEngine.Editor.ImguiEditor.FieldProcessors.Processors
{
    internal sealed class IntFieldProcessor : IUIFieldProcessor
    {
        public bool CanProcess(Type fieldType)
        {
            return fieldType == typeof(int);
        }

        public void Process(ref object value, FieldInfo field, UIAttribute? attribute = null)
        {
            string alias = FieldProcessor.CreateAlias(value, field, attribute);
            var number = (int)value;
            if(ImGui.DragInt(alias, ref number))
            {
                value = number;
            }
        }
    }
}
