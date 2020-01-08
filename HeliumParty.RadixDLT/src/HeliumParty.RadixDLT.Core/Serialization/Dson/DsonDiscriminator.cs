using Dahomey.Cbor;
using Dahomey.Cbor.Serialization;
using Dahomey.Cbor.Serialization.Conventions;
using System;
using System.Collections.Generic;
using System.Text;
using Dahomey.Cbor.Serialization.Converters.Mappings;


namespace HeliumParty.RadixDLT.Serialization.Dson
{
    public class DsonDiscriminator : IDiscriminatorConvention
    {
        private readonly SerializationRegistry _serializationRegistry;
        private readonly ReadOnlyMemory<byte> _memberName = Encoding.ASCII.GetBytes(RadixConstants.SerializerName);
        private readonly Dictionary<string, Type> _typesByDiscriminator = new Dictionary<string, Type>();
        private readonly Dictionary<Type, string> _discriminatorsByType = new Dictionary<Type, string>();

        public DsonDiscriminator(SerializationRegistry serializationRegistry)
        {
            _serializationRegistry = serializationRegistry;
        }

        public ReadOnlySpan<byte> MemberName => _memberName.Span;

        public Type ReadDiscriminator(ref CborReader reader)
        {
            var discriminator = Encoding.UTF8.GetString(reader.ReadRawString().ToArray());
            if (!_typesByDiscriminator.TryGetValue(discriminator, out Type type))
            {
                throw reader.BuildException($"Unknown type discriminator: {discriminator}");
            }
            return type;
        }

        public void WriteDiscriminator(ref CborWriter writer, Type actualType)
        {
            if (!_discriminatorsByType.TryGetValue(actualType, out string discriminator))
            {
                throw new CborException($"Unknown discriminator for type: {actualType}");
            }

            //writer.WriteString(MemberName);
            writer.WriteString(discriminator);
        }
        
        public bool TryRegisterType(Type type)
        {
            IObjectMapping objectMapping;
            try
            {
                 objectMapping = _serializationRegistry.ObjectMappingRegistry.Lookup(type);
            }
            catch
            {
                return false;
            }

            if (objectMapping.Discriminator == null || !(objectMapping.Discriminator is string discriminator))
            {
                return false;
            }

            _discriminatorsByType[type] = discriminator;
            _typesByDiscriminator[discriminator] = type;
            return true;
        }
    }
}