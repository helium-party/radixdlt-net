using System;
using Newtonsoft.Json;

namespace HeliumParty.RadixDLT.Mapping
{
    public class JsonObjectConverter<T> : JsonConverter<T>
    {
        private readonly string _prefix;
        private const int Len = JsonCodecConstants.PrefixLength;

        private readonly Func<T, string> _convertTToString;
        private readonly Func<string, T> _convertStringToT;

        public JsonObjectConverter(Func<T, string> convertTToString, Func<string, T> convertStringToT) : base()
        {
            JsonCodecConstants.JsonPrefixesDictionary.TryGetValue(typeof(T), out _prefix);
            _convertTToString = convertTToString;
            _convertStringToT = convertStringToT;
        }

        public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
        {
            writer.WriteValue(_prefix + _convertTToString(value));
        }

        public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var strValue = (string)reader.Value;

            if (!strValue.StartsWith(_prefix))
                throw new FormatException("Expected prefix: " + _prefix + " but got: " + strValue.Substring(0, Len));

            return _convertStringToT(strValue.Substring(Len));
        }
    }
}