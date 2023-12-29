namespace RockEngine.Common.Editor
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class UIAttribute : Attribute
    {
        public const string UNKNOWN = "##UNKNOWN";
        public string Alias { get; set; }
        public bool IsColor { get; set; }

        public float Speed { get; set; }
        public float Min { get; set; }
        public float Max { get; set; }

        public UIAttribute(string alias = UNKNOWN, bool isColor = false, float speed = 0.01f, float min = float.MinValue, float max = float.MaxValue)
        {
            Alias = alias;
            IsColor = isColor;
            Speed = speed;
            Min = min;
            Max = max;
        }
    }
}
