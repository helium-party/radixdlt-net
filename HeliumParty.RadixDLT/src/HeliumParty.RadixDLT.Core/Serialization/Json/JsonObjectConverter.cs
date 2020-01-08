using System;
using HeliumParty.RadixDLT.EllipticCurve;
using HeliumParty.RadixDLT.Primitives;
using Newtonsoft.Json;

namespace HeliumParty.RadixDLT.Serialization.Json
{
    public class JsonObjectConverter<T> : JsonConverter<T>
    {
        private readonly Func<T, string> _convertTToString;
        private readonly Func<string, T> _convertStringToT;

        private bool _usePrefix = false;
        private string _prefix = ":err:";

        public JsonObjectConverter(Func<T, string> convertTToString, Func<string, T> convertStringToT) : base()
        {
            _convertTToString = convertTToString;
            _convertStringToT = convertStringToT;
            SetPrefix();
        }

        /// <summary>
        /// Sets usePrefix and prefix correctly based on <typeparam>T</typeparam>
        /// </summary>
        protected void SetPrefix()
        {
            var attrs = typeof(T).GetCustomAttributes(typeof(SerializationPrefixAttribute), true);

            if (attrs.Length == 0)
            {
                PrimitivePrefixes(typeof(T));
            }
            else
            {
                var attr = (SerializationPrefixAttribute)attrs[0];
                _usePrefix = attr.HasJsonPrefix;

                _prefix = attr.Json;
            }
        }

        /// <summary>
        /// Sets a prefix for certain primitive types
        /// </summary>
        /// <param name="type"></param>
        private void PrimitivePrefixes(Type type)
        {
            if (type == typeof(byte[]) || type == typeof(ECPrivateKey) || type == typeof(ECPublicKey))
            {
                _prefix = ":byt:";
                _usePrefix = true;
            }

            if (type == typeof(UInt256))
            {
                _prefix = ":u20:";
                _usePrefix = true;
            }

            if (type == typeof(string))
            {
                _prefix = ":str:";
                _usePrefix = true;
            }
        }

        public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var strValue = (string)reader.Value;

            if (!strValue.StartsWith(_prefix) && _usePrefix)
                throw new FormatException("Expected prefix: " + _prefix + " but got: " + strValue.Substring(0, RadixConstants.JsonPrefixLength));

            return _convertStringToT(_usePrefix ? strValue.Substring(RadixConstants.JsonPrefixLength) : strValue);
        }

        public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
        {
            if (_usePrefix)
                writer.WriteValue(_prefix + _convertTToString(value));
            else
                writer.WriteValue(_convertTToString(value));
        }
    }
}