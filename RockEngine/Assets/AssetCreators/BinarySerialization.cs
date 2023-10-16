using OpenTK.Mathematics;

using System.Reflection;

namespace RockEngine.Assets.AssetCreators
{
    internal sealed class BinarySerializer<T> where T : new()
    {
        public T Read(string path)
        {
            using var reader = new BinaryReader(File.Open(path, FileMode.OpenOrCreate));
            return Read(reader);
        }
        public void Write(T obj, string path)
        {
            using var writer = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate));
            Write(obj, writer);
        }
        public T Read(BinaryReader reader)
        {
            var obj = new T();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            SetProperties(reader, obj, properties);
            SetFields(reader, obj, fields);

            return obj;
        }

        public void Write(T obj, BinaryWriter writer)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            GetProperties(obj, writer, properties);
            GetFields(obj, writer, fields);
        }

        private void GetFields(T? obj, BinaryWriter writer, FieldInfo[] fields)
        {
            foreach (var field in fields)
            {
                var type = field.FieldType;

                if (type == typeof(string))
                {
                    writer.Write((string)field.GetValue(obj));
                }
                else if (type == typeof(int))
                {
                    writer.Write((int)field.GetValue(obj));
                }
                else if (type == typeof(float))
                {
                    writer.Write((float)field.GetValue(obj));
                }
                else if (type == typeof(Vector3))
                {
                    var value = (Vector3)field.GetValue(obj);
                    for (int i = 0; i < 3; i++)
                    {
                        writer.Write(value[i]);
                    }
                }
                else if (type == typeof(Vector4))
                {
                    var value = (Vector4)field.GetValue(obj);
                    for (int i = 0; i < 4; i++)
                    {
                        writer.Write(value[i]);
                    }
                }
                else if (type == typeof(Vector2))
                {
                    var value = (Vector2)field.GetValue(obj);
                    for (int i = 0; i < 2; i++)
                    {
                        writer.Write(value[i]);
                    }
                }
                else if (type == typeof(Matrix4))
                {
                    var value = (Matrix4)field.GetValue(obj);
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            writer.Write(value[i, j]);
                        }
                    }
                }
            }
        }

        private static void GetProperties(T obj, BinaryWriter writer, PropertyInfo[] properties)
        {
            foreach (var property in properties)
            {
                var type = property.PropertyType;

                if (type == typeof(string))
                {
                    writer.Write((string)property.GetValue(obj));
                }
                else if (type == typeof(int))
                {
                    writer.Write((int)property.GetValue(obj));
                }
                else if (type == typeof(float))
                {
                    writer.Write((float)property.GetValue(obj));
                }
                else if (type == typeof(Vector3))
                {
                    var value = (Vector3)property.GetValue(obj);
                    for (int i = 0; i < 3; i++)
                    {
                        writer.Write(value[i]);
                    }
                }
                else if (type == typeof(Vector4))
                {
                    var value = (Vector4)property.GetValue(obj);
                    for (int i = 0; i < 4; i++)
                    {
                        writer.Write(value[i]);
                    }
                }
                else if (type == typeof(Vector2))
                {
                    var value = (Vector2)property.GetValue(obj);
                    for (int i = 0; i < 2; i++)
                    {
                        writer.Write(value[i]);
                    }
                }
                else if (type == typeof(Matrix4))
                {
                    var value = (Matrix4)property.GetValue(obj);
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            writer.Write(value[i, j]);
                        }
                    }
                }
            }
        }

        private static void SetProperties(BinaryReader reader, T obj, PropertyInfo[] properties)
        {
            foreach (var property in properties)
            {
                var type = property.PropertyType;

                if (type == typeof(string))
                {
                    property.SetValue(obj, reader.ReadString());
                }
                else if (type == typeof(int))
                {
                    property.SetValue(obj, reader.ReadInt32());
                }
                else if (type == typeof(float))
                {
                    property.SetValue(obj, reader.ReadSingle());
                }
                else if (type == typeof(Vector3))
                {
                    var value = new Vector3();
                    for (int i = 0; i < 3; i++)
                    {
                        value[i] = reader.ReadSingle();
                    }
                    property.SetValue(obj, value);
                }
                else if (type == typeof(Vector4))
                {
                    var value = new Vector4();
                    for (int i = 0; i < 4; i++)
                    {
                        value[i] = reader.ReadSingle();
                    }
                    property.SetValue(obj, value);
                }
                else if (type == typeof(Vector2))
                {
                    var value = new Vector2();
                    for (int i = 0; i < 2; i++)
                    {
                        value[i] = reader.ReadSingle();
                    }
                    property.SetValue(obj, value);
                }
                else if (type == typeof(Matrix4))
                {
                    var value = new Matrix4();
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            value[i, j] = reader.ReadSingle();

                        }
                    }
                    property.SetValue(obj, value);
                }
            }
        }
        private static void SetFields(BinaryReader reader, T obj, FieldInfo[] fields)
        {
            foreach (var field in fields)
            {
                var type = field.FieldType;

                if (type == typeof(string))
                {
                    field.SetValue(obj, reader.ReadString());
                }
                else if (type == typeof(int))
                {
                    field.SetValue(obj, reader.ReadInt32());
                }
                else if (type == typeof(float))
                {
                    field.SetValue(obj, reader.ReadSingle());
                }
                else if (type == typeof(Vector3))
                {
                    var value = new Vector3();
                    for (int i = 0; i < 3; i++)
                    {
                        value[i] = reader.ReadSingle();
                    }
                    field.SetValue(obj, value);
                }
                else if (type == typeof(Vector4))
                {
                    var value = new Vector4();
                    for (int i = 0; i < 4; i++)
                    {
                        value[i] = reader.ReadSingle();
                    }
                    field.SetValue(obj, value);
                }
                else if (type == typeof(Vector2))
                {
                    var value = new Vector2();
                    for (int i = 0; i < 2; i++)
                    {
                        value[i] = reader.ReadSingle();
                    }
                    field.SetValue(obj, value);
                }
                else if (type == typeof(Matrix4))
                {
                    var value = new Matrix4();
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            value[i, j] = reader.ReadSingle();

                        }
                    }
                    field.SetValue(obj, value);
                }
            }
        }
    }
}
