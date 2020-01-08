using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HeliumParty.RadixDLT.EllipticCurve;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HeliumParty.RadixDLT.Serialization.Json
{
    public class JsonContractResolver : DefaultContractResolver
    {
        [SerializationOutputAttribute(OutputMode.None)]
        private readonly OutputMode _outputMode;

        public JsonContractResolver(OutputMode outputMode)
        {
            NamingStrategy = new CamelCaseNamingStrategy();
            _outputMode = outputMode;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);
            var result = new List<JsonProperty>();            

            if (type == typeof(ECSignature))
            {
                result.Add(properties.First(o => o.UnderlyingName == "R"));
                result.Add(properties.First(o => o.UnderlyingName == "S"));
            }
            else if (type == typeof(ECKeyPair))
            {
                properties.First(o => o.UnderlyingName == "PublicKey").PropertyName = "public";
                result.Add(properties.First(o => o.UnderlyingName == "PublicKey"));
                if (_outputMode == OutputMode.Persist)
                {
                    properties.First(o => o.UnderlyingName == "PrivateKey").PropertyName = "private";
                    result.Add(properties.First(o => o.UnderlyingName == "PrivateKey"));
                }
            }
            else
            {
                var props = type.GetProperties().Concat(type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance));

                foreach (var p in props)
                {
                    var serializationAttributes = p.GetCustomAttributes().OfType<SerializationOutputAttribute>();
                    if (!serializationAttributes.Any())
                    {
                        if (_outputMode == OutputMode.None)
                            continue;
                    }
                    else
                    {
                        // Property should be excluded from serialization
                        if (serializationAttributes.Any(a => a.ValidOn.Contains(OutputMode.None)))
                            continue;

                        // For 'OutputMode.All', every property (except the ones with 'OutputMode.None' will be serialized)
                        if (_outputMode != OutputMode.All)
                        {
                            // Check for matching output mode or OutputMode.All
                            if (!serializationAttributes.Any(a => a.ValidOn.Contains(_outputMode) || a.ValidOn.Contains(OutputMode.All)))
                                continue;
                        }
                    }

                    result.Add(properties.First(o => o.UnderlyingName == p.Name));
                }
            }

            return result;
        }
    }
}