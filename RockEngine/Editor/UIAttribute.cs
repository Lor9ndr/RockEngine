namespace RockEngine.Editor
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    internal sealed class UIAttribute : Attribute
    {
        public const string UNKNOWN = "##UNKNOWN";
        public string Alias { get; set; }
        public bool IsColor { get; set; }

        public UIAttribute(string alias = UNKNOWN, bool isColor = false)
        {
            Alias = alias;
            IsColor = isColor;
        }
    }
}
