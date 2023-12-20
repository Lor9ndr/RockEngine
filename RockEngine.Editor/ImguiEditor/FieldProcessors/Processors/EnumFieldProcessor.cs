using ImGuiNET;

using System.Reflection;

namespace RockEngine.Editor.ImguiEditor.FieldProcessors.Processors
{
    internal sealed class EnumFieldProcessor : IUIFieldProcessor
    {
        public  bool CanProcess(Type fieldType)
        {
            return fieldType.BaseType == typeof(Enum);
        }

        public void Process(ref object obj, ref object value, FieldInfo field, UIAttribute? attribute = null)
        {
            string alias = FieldProcessor.CreateAlias(obj, field, attribute);
            var type = value.GetType();
            var names = Enum.GetNames(type);
            var values = Enum.GetValues(type);
            var selectedIndex = Array.IndexOf(values, value);

            if(ImGui.Combo(alias, ref selectedIndex, names, names.Length))
            {
                value = values.GetValue(selectedIndex);
            }
        }
    }
}
