using OpenTK.Mathematics;
using System.Text;

namespace RockEngine.Utils
{
    internal sealed class ColoredLog
    {
        private readonly List<Record> _records;
        private readonly StringBuilder _currentMessage;
        private Vector4 _currentColor;

        public ColoredLog()
        {
            _records = new List<Record>();
            _currentMessage = new StringBuilder();
        }

        public void Append(string message, Vector4 color)
        {
            if(_currentColor == color)
            {
                _currentMessage.AppendLine(message);
            }
            else
            {
                AddCurrentRecord();
                _currentMessage.AppendLine(message);
                _currentColor = color;
            }
        }

        public void AddLog(string message)
        {
            Append(message, Record.Log);
        }

        public void AddError(string message)
        {
            Append(message, Record.Error);
        }

        public void AddWarn(string message)
        {
            Append(message, Record.Warn);
        }

        public void Clear()
        {
            _records.Clear();
            _currentMessage.Clear();
        }

        public int Count => _records.Count;
        public List<Record> GetRecords()
        {
            AddCurrentRecord();
            return _records;
        }

        private void AddCurrentRecord()
        {
            if(_currentMessage.Length != 0)
            {
                var record = new Record(_currentMessage.ToString(), _currentColor);
                _records.Add(record);
                _currentMessage.Clear();
            }
        }

        internal struct Record
        {
            public string Text { get; }
            public Vector4 Color { get; }

            public Record(string text, Vector4 color)
            {
                Text = text;
                Color = color;
            }

            public static Vector4 Error => Vector4.UnitX with { W = 1 };
            public static Vector4 Warn => new Vector4(1f, 1, 0, 1);
            public static Vector4 Log => Vector4.UnitY with { W = 1 };
        }
    }
}