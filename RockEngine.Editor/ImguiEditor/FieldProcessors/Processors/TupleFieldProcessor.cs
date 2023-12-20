using ImGuiNET;

using System.Reflection;
using System.Runtime.CompilerServices;

namespace RockEngine.Editor.ImguiEditor.FieldProcessors.Processors
{
    internal sealed class TupleFieldProcessor : IUIFieldProcessor
    {
        public bool CanProcess(Type fieldType)
        {
            return fieldType == typeof(ITuple);
        }

        public void Process(ref object obj, ref object value, FieldInfo field, UIAttribute? attribute = null)
        {
            string alias = FieldProcessor.CreateAlias(obj, field, attribute);
            ImGui.Text(alias);
            var tuple = (ITuple)value;
            for(int i = 0; i < tuple.Length; i++)
            {
                var item = tuple[i];
                // Process each item in the tuple
                FieldProcessor.ProcessField(ref item);
            }
        }
    }
}
