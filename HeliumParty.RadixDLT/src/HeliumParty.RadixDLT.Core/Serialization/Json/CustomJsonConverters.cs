using System;
using HeliumParty.RadixDLT.EllipticCurve;
using HeliumParty.RadixDLT.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HeliumParty.RadixDLT.Serialization.Json
{
    public class ECKeypairJsonConverters : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);

            var pubKey = new ECPublicKey(((string)jo["public"]).Substring(RadixConstants.JsonPrefixLength));

            ECPrivateKey privKey = null;
            try
            {
                privKey = new ECPrivateKey(((string)jo["private"]).Substring(RadixConstants.JsonPrefixLength));
            }
            catch
            {
                return new ECKeyPair(null, pubKey);
            }

            return new ECKeyPair(privKey, pubKey);
        }

        public override bool CanConvert(Type objectType) => objectType == typeof(ECKeyPair);

        public override bool CanWrite => false;
    }

    public class ECSignatureJsonConverters : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);

            var r = Bytes.FromBase64String(((string)jo["r"]).Substring(RadixConstants.JsonPrefixLength));
            var s = Bytes.FromBase64String(((string)jo["s"]).Substring(RadixConstants.JsonPrefixLength));

            return new ECSignature(r, s);
        }

        public override bool CanConvert(Type objectType) => objectType == typeof(ECSignature);

        public override bool CanWrite => false;
    }
}