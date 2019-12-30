using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dahomey.Cbor.Attributes;
using HeliumParty.Collections;
using HeliumParty.RadixDLT.EllipticCurve;
using Newtonsoft.Json.Serialization;

namespace HeliumParty.RadixDLT.Serialization.Json
{
    public class JsonSerializationBinder : DefaultSerializationBinder
    {
        private readonly Dictionary<string, Type> _typesByDiscriminator = new Dictionary<string, Type>();
        private readonly Dictionary<Type, string> _discriminatorsByType = new Dictionary<Type, string>();
        private readonly HashSet<Type> _discriminatedTypes = new HashSet<Type>();

        public JsonSerializationBinder()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                RegisterAssembly(assembly);
            }

            // Register Types below Core Layer manually
            RegisterType(typeof(ECKeyPair), "crypto.ec_key_pair");
            RegisterType(typeof(ECSignature), "crypto.ecdsa_signature");
        }

        public void RegisterAssembly(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes()
                .Where(t => Attribute.IsDefined(t, typeof(CborDiscriminatorAttribute))))
            {
                var discriminator = type.GetCustomAttribute<CborDiscriminatorAttribute>().Discriminator;
                RegisterType(type, discriminator);
            }
        }

        public void RegisterType(Type type, string discriminator)
        {
            _typesByDiscriminator[discriminator] = type;
            _discriminatorsByType[type] = discriminator;

            // mark all base types as discriminated (so we know that it's worth reading a discriminator)
            for (Type baseType = type.BaseType; baseType != null; baseType = baseType.BaseType)
            {
                _discriminatedTypes.AddIfNotContains(baseType);
            }
        }

        public override Type BindToType(string assemblyName, string typeName)
        {
            return _typesByDiscriminator[typeName];
        }

        public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = null;
            var success = _discriminatorsByType.TryGetValue(serializedType, out typeName);

            if (!success)
                typeName = null;
        }
    }
}