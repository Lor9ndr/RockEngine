using ImGuiNET;

using RockEngine.Common.Editor;

using System.Reflection;

namespace RockEngine.Editor.ImguiEditor.FieldProcessors.Processors
{
    internal sealed class BoolFieldProcessor : IUIFieldProcessor
    {
        public bool CanProcess(Type fieldType)
        {
            return fieldType == typeof(bool);
        }

        public void Process(ref object value, FieldInfo field, UIAttribute attribute)
        {
            string alias = FieldProcessor.CreateAlias(value, field, attribute);
            bool obj = (bool)value;
            if(ImGui.Checkbox(alias, ref obj))
            {
                value = obj;
            }
        }
    }
}
