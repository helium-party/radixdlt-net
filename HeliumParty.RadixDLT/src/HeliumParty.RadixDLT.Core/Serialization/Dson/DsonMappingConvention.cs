using Dahomey.Cbor.Serialization;
using Dahomey.Cbor.Serialization.Conventions;
using Dahomey.Cbor.Serialization.Converters.Mappings;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT.Serialization.Dson
{
    public class DsonNamingConvention : INamingConvention
    {
        public string GetPropertyName(string name)
        {
            return name.ToLowerInvariant();
        }
    }
    public class DsonObjectMappingConvention : IObjectMappingConvention
    {
        private readonly IObjectMappingConvention defaultObjectMappingConvention = new DefaultObjectMappingConvention();
        private readonly INamingConvention DsonNamingConvention = new DsonNamingConvention();

        public void Apply<T>(SerializationRegistry registry, ObjectMapping<T> objectMapping) where T : class
        {
            defaultObjectMappingConvention.Apply<T>(registry, objectMapping);
            objectMapping.SetNamingConvention(DsonNamingConvention);
        }
    }

    public class DsonObjectMappingConventionProvider : IObjectMappingConventionProvider
    {
        private readonly IObjectMappingConvention _objectMappingConvention = new DsonObjectMappingConvention();

        public IObjectMappingConvention GetConvention(Type type)
        {
            return _objectMappingConvention;
        }
    }
}
