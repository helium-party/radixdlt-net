using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using Dahomey.Cbor;
using Dahomey.Cbor.Attributes;
using Dahomey.Cbor.Serialization;
using Dahomey.Cbor.Serialization.Conventions;
using Dahomey.Cbor.Serialization.Converters.Mappings;
using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.EllipticCurve;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Particles.Types;

namespace HeliumParty.RadixDLT.Mapping
{
    public static class DsonOutputMapping
    {
        private static Dictionary<OutputMode, CborOptions> _outputModeOptions = new Dictionary<OutputMode, CborOptions>();

        static DsonOutputMapping()
        {
            // registering custom dson converters
            CborOptions.Default.Registry.ConverterRegistry.RegisterConverter(typeof(byte[]), new DsonObjectConverter<byte[]>(x => x, y => y));
            CborOptions.Default.Registry.ConverterRegistry.RegisterConverter(typeof(EUID), new DsonObjectConverter<EUID>(x => x.ToByteArray(), y => new EUID(y)));
            //CborOptions.Default.Registry.ConverterRegistry.RegisterConverter(typeof(Hash), new DsonObjectConverter<Hash>(x => x.ToByteArray(), y => new Hash(y)));
            CborOptions.Default.Registry.ConverterRegistry.RegisterConverter(typeof(RadixAddress), new DsonObjectConverter<RadixAddress>(x => Base58Encoding.Decode(x.ToString()), y => new RadixAddress(Base58Encoding.Encode(y))));
            //CborOptions.Default.Registry.ConverterRegistry.RegisterConverter(typeof(UInt256), new DsonObjectConverter<UInt256>(x => x.ToByteArray(), UInt256.From));
            CborOptions.Default.Registry.ConverterRegistry.RegisterConverter(typeof(RRI), new DsonObjectConverter<RRI>(x => RadixConstants.StandardEncoding.GetBytes(x.ToString()), y => new RRI(RadixConstants.StandardEncoding.GetString(y))));
            //CborOptions.Default.Registry.ConverterRegistry.RegisterConverter(typeof(UInt384), new DsonObjectConverter<UInt384>());
            CborOptions.Default.Registry.ConverterRegistry.RegisterConverter(typeof(AID), new DsonObjectConverter<AID>(x => x.Bytes, y => new AID(y)));

            CborOptions.Default.Registry.ObjectMappingConventionRegistry.RegisterProvider(new OptInObjectMappingConventionProvider());

            // initialize the default config for each output mode
            foreach (var mode in (OutputMode[])Enum.GetValues(typeof(OutputMode)))
                _outputModeOptions.Add(mode, CborOptions.Default);
            _outputModeOptions[OutputMode.None] = null; // TODO do we even require output mode none?

            // register output mode mappings

            #region Hash

            _outputModeOptions[OutputMode.Hash].Registry.ObjectMappingRegistry.Register<Atom>(om =>
            {
                om.AutoMap();
                om.ClearMemberMappings();
                om.MapMember(o => o.ParticleGroups);
                om.MapMember(o => o.MetaData);
                om.MapMember(o => o.Id);
            });
            _outputModeOptions[OutputMode.Hash].Registry.ObjectMappingRegistry.Register<ECKeyPair>(om =>
            {
                om.AutoMap();
                om.ClearMemberMappings();
                om.MapMember(o => o.PublicKey);
            });

            #endregion

            #region Api

            _outputModeOptions[OutputMode.Api].Registry.ObjectMappingRegistry.Register<ECKeyPair>(om =>
            {
                om.AutoMap();
                om.ClearMemberMappings();
                om.MapMember(o => o.PublicKey);
            });

            #endregion

            #region Wire

            _outputModeOptions[OutputMode.Wire].Registry.ObjectMappingRegistry.Register<ECKeyPair>(om =>
            {
                om.AutoMap();
                om.ClearMemberMappings();
                om.MapMember(o => o.PublicKey);
            });

            #endregion

            #region Persist



            #endregion

        }

        public static CborOptions GetCborOptions(OutputMode mode)
        {
            if (mode == null)
                throw new ArgumentNullException(nameof(mode));

            return _outputModeOptions[mode];
        }

        public class OptInObjectMappingConventionProvider : IObjectMappingConventionProvider
        {
            public IObjectMappingConvention GetConvention(Type type)
            {
                // here you could filter which type should be optIn and return null for other types
                return new OptInObjectMappingConvention();
            }
        }
        public class OptInObjectMappingConvention : IObjectMappingConvention
        {
            private readonly DefaultObjectMappingConvention _defaultConvention = new DefaultObjectMappingConvention();

            public void Apply<T>(SerializationRegistry registry, ObjectMapping<T> objectMapping) where T : class
            {
                _defaultConvention.Apply(registry, objectMapping);

                // restrict to members holding CborPropertyAttribute
                objectMapping.SetMemberMappings(objectMapping.MemberMappings
                    .Where(m => m.MemberInfo.IsDefined(typeof(CborPropertyAttribute), true)).ToList());
            }
        }
    }
}