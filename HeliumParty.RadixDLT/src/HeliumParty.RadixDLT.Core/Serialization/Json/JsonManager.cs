using System;
using System.Collections.Generic;
using System.Numerics;
using HeliumParty.DependencyInjection;
using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.EllipticCurve;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Primitives;
using Newtonsoft.Json;

namespace HeliumParty.RadixDLT.Serialization.Json
{
    public class JsonManager : IJsonManager, ITransientDependency
    {
        private readonly Dictionary<OutputMode, JsonSerializerSettings> _outputModeOptions;
        private const string Discriminator = "serializer";

        public JsonManager()
        {
            _outputModeOptions = new Dictionary<OutputMode, JsonSerializerSettings>();
            InitializeOptions();
        }

        public T FromJson<T>(string str, OutputMode mode = OutputMode.All)
        {
            var json = str.Replace(Discriminator, "$type"); // TODO ugly workaround, https://github.com/JamesNK/Newtonsoft.Json/issues/36 COULD CAUSE PROBLEMS!
            return JsonConvert.DeserializeObject<T>(json, GetJsonSettings(mode));
        }

        public string ToJson<T>(T obj, OutputMode mode = OutputMode.All)
        {
            var json = JsonConvert.SerializeObject(obj, GetJsonSettings(mode));
            return json.Replace("$type", Discriminator); // TODO ugly workaround, https://github.com/JamesNK/Newtonsoft.Json/issues/36 COULD CAUSE PROBLEMS!
        }

        protected virtual JsonSerializerSettings GetJsonSettings(OutputMode mode) => _outputModeOptions[mode];

        protected virtual void InitializeOptions()
        {
            var jsonConverters = new JsonConverterCollection
            {
                new JsonObjectConverter<byte[]>(Bytes.ToBase64String, Bytes.FromBase64String),
                new JsonObjectConverter<UInt256>(x => x.ToString(), y => y),
                new JsonObjectConverter<EUID>(x => x.ToString(), y => new EUID(y)),
                new JsonObjectConverter<ECPrivateKey>(x => x.ToString(), y => new ECPrivateKey(y)),
                new JsonObjectConverter<ECPublicKey>(x => x.ToString(), y => new ECPublicKey(y)),
                new JsonObjectConverter<BigInteger>(x => x.ToString(), BigInteger.Parse),
                new JsonObjectConverter<string>(x => x, y => y),
                new JsonObjectConverter<RadixAddress>(x => x.ToString(), y => new RadixAddress(y)),
                new JsonObjectConverter<RRI>(x => x.ToString(), y => new RRI(y)),
                new JsonObjectConverter<AID>(x => x.ToString(), y => new AID(y)),

                new ECKeypairJsonConverters(),
                new ECSignatureJsonConverters()
            };

            foreach (var mode in (OutputMode[]) Enum.GetValues(typeof(OutputMode)))
            {
                var settings = new JsonSerializerSettings();
                settings.Converters = jsonConverters;
                settings.Formatting = Formatting.None;
                settings.ContractResolver = new JsonContractResolver(mode);
                settings.SerializationBinder = new JsonSerializationBinder();
                settings.TypeNameHandling = TypeNameHandling.Objects;
                settings.MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead;
                _outputModeOptions.Add(mode, settings);
            }
        }
    }
}