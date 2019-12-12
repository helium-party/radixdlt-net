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
            var props = type.GetProperties().Concat(type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance)).ToArray();

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
                foreach (var p in props)
                {
                    var shouldSerialize = true;

                    if (p.GetCustomAttributes().Where(x => x.GetType() == typeof(SerializationOutputAttribute)).ToArray().Length == 0)
                    {
                        // field or property has no attribute
                        switch (_outputMode)
                        {
                            case OutputMode.None:
                                shouldSerialize = false; // to archive zero output when OutputMode == None
                                break;
                            case OutputMode.Hash:
                                break;
                            case OutputMode.Api:
                                break;
                            case OutputMode.Wire:
                                break;
                            case OutputMode.Persist:
                                break;
                            case OutputMode.All:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    else
                    {
                        // Attribute is present
                        foreach (var attr in p.GetCustomAttributes())
                        {
                            if (attr.GetType() != typeof(SerializationOutputAttribute)) continue;

                            var oa = (SerializationOutputAttribute)attr;
                            switch (_outputMode)
                            {
                                case OutputMode.None:
                                    {
                                        shouldSerialize = false;
                                        break;
                                    }
                                case OutputMode.Hash:
                                    {
                                        if (oa.ValidOn.Contains(OutputMode.Hash) || oa.ValidOn.Contains(OutputMode.All))
                                            shouldSerialize = true;
                                        else shouldSerialize = false;

                                        break;
                                    }
                                case OutputMode.Api:
                                    {
                                        if (oa.ValidOn.Contains(OutputMode.Api) || oa.ValidOn.Contains(OutputMode.All))
                                            shouldSerialize = true;
                                        else shouldSerialize = false;

                                        break;
                                    }
                                case OutputMode.Wire:
                                    {
                                        if (oa.ValidOn.Contains(OutputMode.Wire) || oa.ValidOn.Contains(OutputMode.All))
                                            shouldSerialize = true;
                                        else shouldSerialize = false;

                                        break;
                                    }
                                case OutputMode.Persist:
                                    {
                                        if (oa.ValidOn.Contains(OutputMode.Persist) || oa.ValidOn.Contains(OutputMode.All))
                                            shouldSerialize = true;
                                        else shouldSerialize = false;

                                        break;
                                    }
                                case OutputMode.All:
                                    {
                                        shouldSerialize = true;
                                        if (oa.ValidOn.Contains(OutputMode.None))
                                            shouldSerialize = false;

                                        break;
                                    }
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                    }

                    if (shouldSerialize)
                    {
                        result.Add(properties.First(o => o.UnderlyingName == p.Name));
                    }
                }
            }

            return result;
        }
    }
}