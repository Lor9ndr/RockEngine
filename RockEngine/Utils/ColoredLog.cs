using OpenTK.Mathematics;

using System.Collections;
using System.Text;

namespace RockEngine.Utils
{
    internal sealed class ColoredLog : IEnumerable<ColoredLog.Record>
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
            if (_currentColor == color)
            {
                _currentMessage.AppendLine(message);
            }
            else
            {
                _records.Add(new Record(_currentMessage.ToString(), _currentColor));
                _currentMessage.Clear();
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
            => _records.Clear();

        public IEnumerator<Record> GetEnumerator()
        {
            // при добавлении мы не добавляем текущее сообщение, если цвет его схож с предыдущим, в общем пропуск бывает
            if (_currentMessage.Length != 0)
            {
                _records.Add(new Record(_currentMessage.ToString(), _currentColor));
                _currentMessage.Clear();
            }
            return _records.GetEnumerator();

        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        internal record struct Record
        {
            public string Text;
            public Vector4 Color;

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
