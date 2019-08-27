using System;
using System.Collections.Generic;
using System.Linq;
using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.EllipticCurve;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HeliumParty.RadixDLT.Mapping
{
    public class JsonOutputContract : DefaultContractResolver
    {
        private readonly OutputMode _outputMode;

        public JsonOutputContract(OutputMode outputMode)
        {
            _outputMode = outputMode;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);

            switch (_outputMode)
            {
                case OutputMode.Hash:
                {
                    if (type == typeof(Atom))
                        properties = properties.Where(p => p.PropertyName != "signatures").ToList();
                    if (type == typeof(ECKeyPair))
                        properties = properties.Where(p => p.PropertyName != "private").ToList();
                    break;
                }
                case OutputMode.Api:
                {
                    if (type == typeof(ECKeyPair))
                        properties = properties.Where(p => p.PropertyName != "private").ToList();
                    break;
                }
                case OutputMode.Wire:
                {
                    if (type == typeof(ECKeyPair))
                        properties = properties.Where(p => p.PropertyName != "private").ToList();
                    break;
                }
                case OutputMode.Persist:
                {
                    break;
                }
                case OutputMode.All:
                    break;
            }

            return properties;
        }
    }
}