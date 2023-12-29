using ImGuiNET;

using RockEngine.Common.Editor;

using System.Reflection;

namespace RockEngine.Editor.ImguiEditor.FieldProcessors.Processors
{
    internal sealed class StringFieldProcessor : IUIFieldProcessor
    {
        public bool CanProcess(Type fieldType)
        {
            return fieldType == typeof(string);
        }

        public void Process(ref object value, FieldInfo field, UIAttribute attribute)
        {
            string str = (string)value;
            var alias = FieldProcessor.CreateAlias(value, field, attribute);
            if(ImGui.InputText(alias, ref str, 999))
            {
                value = str;
            }
        }
    }
}
