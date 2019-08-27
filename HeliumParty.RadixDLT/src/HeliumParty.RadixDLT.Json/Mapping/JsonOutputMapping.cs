using System;
using System.Collections.Generic;
using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Primitives;
using Newtonsoft.Json;

namespace HeliumParty.RadixDLT.Mapping
{
    public class JsonOutputMapping
    {
        private static Dictionary<OutputMode, JsonSerializerSettings> _outputModeOptions = new Dictionary<OutputMode, JsonSerializerSettings>();

        static JsonOutputMapping()
        {
            var jsonConverters = new JsonConverterCollection
            {
                new JsonObjectConverter<byte[]>(Bytes.ToBase64String, Bytes.FromBase64String),
                new JsonObjectConverter<EUID>(x => x.ToString(), y => new EUID(y)),
                //new JsonObjectConverter<Hash>(x => x.ToString(), y => new Hash(y)),
                new JsonObjectConverter<string>(x => x, y => y),
                new JsonObjectConverter<RadixAddress>(x => x.ToString(), y => new RadixAddress(y)),
                //new JsonObjectConverter<UInt256>(x => x.ToString(), UInt256.From),
                new JsonObjectConverter<RRI>(x => x.ToString(), y => new RRI(y)),
                //new JsonObjectConverter<UInt384>(),
                new JsonObjectConverter<AID>(x => x.ToString(), y => new AID(y)),
            };

            foreach (var mode in (OutputMode[])Enum.GetValues(typeof(OutputMode)))
            {
                var settings = new JsonSerializerSettings();
                settings.Converters = jsonConverters;
                settings.ContractResolver = new JsonOutputContract(mode);
                settings.Formatting = Formatting.None;
                
                _outputModeOptions.Add(mode, settings);
            }
        }

        public static JsonSerializerSettings GetJsonOptions(OutputMode mode) => _outputModeOptions[mode];
    }
}