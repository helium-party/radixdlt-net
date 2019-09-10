using Dahomey.Cbor.Serialization;
using Dahomey.Cbor.Serialization.Conventions;
using Dahomey.Cbor.Serialization.Converters.Mappings;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace HeliumParty.RadixDLT.Serialization.Dson
{
    public class DsonObjectMappingConvention : IObjectMappingConvention
    {
        private readonly IObjectMappingConvention defaultObjectMappingConvention = new DefaultObjectMappingConvention();
        private readonly INamingConvention DsonNamingConvention = new CamelCaseNamingConvention();
        private readonly OutputMode _outputMode;
        
        public DsonObjectMappingConvention(OutputMode outputMode)
        {
            _outputMode = outputMode;
        }

        public void Apply<T>(SerializationRegistry registry, ObjectMapping<T> objectMapping) where T : class
        {
            defaultObjectMappingConvention.Apply<T>(registry, objectMapping);

            objectMapping.ClearMemberMappings();
            var props = typeof(T).GetProperties();
            foreach (var p in props)
            {

                var shouldSerialize = true;
                foreach (var attr in p.GetCustomAttributes())
                {
                    if (attr.GetType() == typeof(SerializationOutputAttribute))
                    {
                        var oa = (SerializationOutputAttribute)attr;

                        switch (_outputMode)
                        {
                            case OutputMode.Hash:
                                {
                                    if (oa.ValidOn == OutputMode.Hash)
                                        shouldSerialize = true;
                                    else shouldSerialize = false;

                                    break;
                                }
                            case OutputMode.All:
                                {
                                    shouldSerialize = true;
                                    if (oa.ValidOn == OutputMode.Hidden)
                                        shouldSerialize = false;
                                    break;
                                }
                        }
                    }
                    else
                    {
                        switch (_outputMode)
                        {
                            case OutputMode.Hash:
                                {
                                    shouldSerialize = false;
                                    break;
                                }
                            default:
                                {
                                    shouldSerialize = true;
                                    break;
                                }
                        }
                    }
                }

                if (shouldSerialize)
                {
                    objectMapping.MapMember(p);
                }

            }
            
            objectMapping.SetNamingConvention(DsonNamingConvention); 
            
        }
    }

    public class DsonObjectMappingConventionProvider : IObjectMappingConventionProvider
    {
        private readonly IObjectMappingConvention _objectMappingConvention;

        public DsonObjectMappingConventionProvider(OutputMode mode)
        {
            _objectMappingConvention = new DsonObjectMappingConvention(mode);
        }

        public IObjectMappingConvention GetConvention(Type type)
        {
            return _objectMappingConvention;
        }
    }
}
