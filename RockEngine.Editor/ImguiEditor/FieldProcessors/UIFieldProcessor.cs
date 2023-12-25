using System.Reflection;

namespace RockEngine.Editor.ImguiEditor.FieldProcessors
{
    public interface IUIFieldProcessor
    {
        bool CanProcess(Type fieldType);
        void Process(ref object value, FieldInfo field, UIAttribute? attribute = null);
    }
}
