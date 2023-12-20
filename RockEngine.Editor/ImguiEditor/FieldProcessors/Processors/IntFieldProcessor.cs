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

        public void Process(ref object obj, ref object value, FieldInfo field, UIAttribute? attribute = null)
        {
            string alias = FieldProcessor.CreateAlias(obj, field, attribute);
            var setter = FieldProcessor.GetOrCreateFieldSetter(field);
            var number = (int)value;
            if(ImGui.DragInt(alias, ref number))
            {
                setter(obj, number);
            }
        }
    }
}
