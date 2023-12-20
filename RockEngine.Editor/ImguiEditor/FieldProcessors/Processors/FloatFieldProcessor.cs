using ImGuiNET;

using System.Reflection;

namespace RockEngine.Editor.ImguiEditor.FieldProcessors.Processors
{
    internal sealed class FloatFieldProcessor : IUIFieldProcessor
    {
        public bool CanProcess(Type fieldType)
        {
            return fieldType == typeof(float);
        }

        public void Process(ref object obj, ref object value, FieldInfo field, UIAttribute? attribute = null)
        {
            string alias = FieldProcessor.CreateAlias(obj, field, attribute);
            var number = (float)value;
            if(ImGui.DragFloat(alias, ref number))
            {
                value = number;
            }
        }
    }
}
