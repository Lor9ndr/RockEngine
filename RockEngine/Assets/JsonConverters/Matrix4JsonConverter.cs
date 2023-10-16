using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using OpenTK.Mathematics;

namespace RockEngine.Assets.JsonConverters
{
    internal sealed class Matrix4JsonConverter : JsonConverter<Matrix4>
    {
        public override Matrix4 ReadJson(JsonReader reader, Type objectType, Matrix4 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // Load the JSON object
            JObject jsonObject = JObject.Load(reader);

            // Extract the elements of the matrix from the JSON object
            float m11 = jsonObject["m11"]!.ToObject<float>();
            float m12 = jsonObject["m12"]!.ToObject<float>();
            float m13 = jsonObject["m13"]!.ToObject<float>();
            float m14 = jsonObject["m14"]!.ToObject<float>();
            float m21 = jsonObject["m21"]!.ToObject<float>();
            float m22 = jsonObject["m22"]!.ToObject<float>();
            float m23 = jsonObject["m23"]!.ToObject<float>();
            float m24 = jsonObject["m24"]!.ToObject<float>();
            float m31 = jsonObject["m31"]!.ToObject<float>();
            float m32 = jsonObject["m32"]!.ToObject<float>();
            float m33 = jsonObject["m33"]!.ToObject<float>();
            float m34 = jsonObject["m34"]!.ToObject<float>();
            float m41 = jsonObject["m41"]!.ToObject<float>();
            float m42 = jsonObject["m42"]!.ToObject<float>();
            float m43 = jsonObject["m43"]!.ToObject<float>();
            float m44 = jsonObject["m44"]!.ToObject<float>();

            // Create and return a new Matrix4 object
            return new Matrix4(m11, m12, m13, m14, m21, m22, m23, m24, m31, m32, m33, m34, m41, m42, m43, m44);
        }

        public override void WriteJson(JsonWriter writer, Matrix4 value, JsonSerializer serializer)
        {
            // Create a JSON object with the elements of the matrix
            JObject jsonObject = new JObject();
            jsonObject["m11"] = value.M11;
            jsonObject["m12"] = value.M12;
            jsonObject["m13"] = value.M13;
            jsonObject["m14"] = value.M14;
            jsonObject["m21"] = value.M21;
            jsonObject["m22"] = value.M22;
            jsonObject["m23"] = value.M23;
            jsonObject["m24"] = value.M24;
            jsonObject["m31"] = value.M31;
            jsonObject["m32"] = value.M32;
            jsonObject["m33"] = value.M33;
            jsonObject["m34"] = value.M34;
            jsonObject["m41"] = value.M41;
            jsonObject["m42"] = value.M42;
            jsonObject["m43"] = value.M43;
            jsonObject["m44"] = value.M44;

            // Write the JSON object to the writer
            jsonObject.WriteTo(writer);
        }
    }
}
