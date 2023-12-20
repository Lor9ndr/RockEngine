using ImGuiNET;

using System.Reflection;

namespace RockEngine.Editor.ImguiEditor.FieldProcessors.Processors
{
    internal sealed class StringFieldProcessor : IUIFieldProcessor
    {
        public bool CanProcess(Type fieldType)
        {
            return fieldType == typeof(string);
        }

        public void Process(ref object refToValueHandler, ref object value, FieldInfo field, UIAttribute? attribute = null)
        {
            string str = (string)value;
            if(ImGui.InputText(field.Name, ref str, 999))
            {
                value = str;
            }
        }
    }
}
