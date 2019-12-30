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
                var props = type
                    .GetProperties()
                    .Concat(type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance));

                bool shouldSerialize;
                foreach (var p in props)
                {
                    // By default, we always want to serialize the property (e.g. if it doesn't have an OutputAttribute)
                    shouldSerialize = true;

                    var serializationAttributes = p.GetCustomAttributes().OfType<SerializationOutputAttribute>();
                    if (serializationAttributes.Count() == 0)
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
                                shouldSerialize = false;
                        }
                    }

                    if (shouldSerialize)
                        result.Add(properties.First(o => o.UnderlyingName == p.Name));
                }
            }

            return result;
        }
    }
}