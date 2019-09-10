using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dahomey.Cbor;
using Dahomey.Cbor.Attributes;
using Dahomey.Cbor.Serialization;
using Dahomey.Cbor.Serialization.Converters.Mappings;
using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.EllipticCurve;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Particles;
using HeliumParty.RadixDLT.Primitives;
using Org.BouncyCastle.Math;

namespace HeliumParty.RadixDLT.Serialization.Dson
{
    public static class DsonOutputMapping
    {
        private static Dictionary<OutputMode, CborOptions> _outputModeOptions = new Dictionary<OutputMode, CborOptions>();

        static DsonOutputMapping()
        {
            //CborOptions.Default.Registry.ObjectMappingConventionRegistry.RegisterProvider(new DsonObjectMappingConventionProvider());
            //CborOptions.Default.Registry.ConverterRegistry.RegisterConverter(typeof(byte[]), new DsonObjectConverter<byte[]>(x => x, y => y));
            //CborOptions.Default.Registry.ConverterRegistry.RegisterConverter(typeof(EUID), new DsonObjectConverter<EUID>(x => x.ToByteArray(), y => new EUID(y)));
            

            // initialize the default config for each output mode
            foreach (var mode in (OutputMode[])Enum.GetValues(typeof(OutputMode)))
            {
                var discriminator = new DsonDiscriminator();
                discriminator.RegisterAssembly(Assembly.GetAssembly(typeof(Particle)));


                var options = new CborOptions();
                options.Registry.ObjectMappingConventionRegistry.RegisterProvider(new DsonObjectMappingConventionProvider(mode));
                options.DiscriminatorConvention = discriminator;

                options.Registry.ConverterRegistry.RegisterConverter(typeof(byte[]), new DsonObjectConverter<byte[]>(x => x, y => y));
                //options.Registry.ConverterRegistry.RegisterConverter(typeof(UInt256), new DsonObjectConverter<byte[]>(x => x, y => y));
                options.Registry.ConverterRegistry.RegisterConverter(typeof(EUID), new DsonObjectConverter<EUID>(x => x.ToByteArray(), y => new EUID(y)));
                options.Registry.ConverterRegistry.RegisterConverter(typeof(ECPrivateKey),new DsonObjectConverter<ECPrivateKey>(x => x.Base64Array, y => new ECPrivateKey(y)));
                options.Registry.ConverterRegistry.RegisterConverter(typeof(ECPublicKey),new DsonObjectConverter<ECPublicKey>(x => x.Base64Array, y => new ECPublicKey(y)));
                options.Registry.ConverterRegistry.RegisterConverter(typeof(BigInteger), new DsonObjectConverter<BigInteger>(x => x.ToByteArray(), y => new BigInteger(1,y)));
                options.Registry.ConverterRegistry.RegisterConverter(typeof(RadixAddress), new DsonObjectConverter<RadixAddress>(x => Base58Encoding.Decode(x.ToString()), y => new RadixAddress(Base58Encoding.Encode(y))));
                options.Registry.ConverterRegistry.RegisterConverter(typeof(RRI), new DsonObjectConverter<RRI>(x => RadixConstants.StandardEncoding.GetBytes(x.ToString()), y => new RRI(RadixConstants.StandardEncoding.GetString(y))));
                options.Registry.ConverterRegistry.RegisterConverter(typeof(AID), new DsonObjectConverter<AID>(x => x.Bytes, y => new AID(y)));


                //hide the private key
                options.Registry.ObjectMappingRegistry.Register<ECKeyPair>(om => 
                {
                    om.ClearMemberMappings();
                    om.MapMember(m => m.PublicKey);
                });

                
                

                _outputModeOptions.Add(mode, options);
            }
        }

        //static DsonOutputMapping()
        //{
        //    // registering custom dson converters
        //    CborOptions.Default.Registry.ConverterRegistry.RegisterConverter(typeof(byte[]), new DsonObjectConverter<byte[]>(x => x, y => y));
        //    CborOptions.Default.Registry.ConverterRegistry.RegisterConverter(typeof(EUID), new DsonObjectConverter<EUID>(x => x.ToByteArray(), y => new EUID(y)));
        //    //CborOptions.Default.Registry.ConverterRegistry.RegisterConverter(typeof(Hash), new DsonObjectConverter<Hash>(x => x.ToByteArray(), y => new Hash(y)));
        //    CborOptions.Default.Registry.ConverterRegistry.RegisterConverter(typeof(RadixAddress), new DsonObjectConverter<RadixAddress>(x => Base58Encoding.Decode(x.ToString()), y => new RadixAddress(Base58Encoding.Encode(y))));
        //    //CborOptions.Default.Registry.ConverterRegistry.RegisterConverter(typeof(UInt256), new DsonObjectConverter<UInt256>(x => x.ToByteArray(), UInt256.From));
        //    CborOptions.Default.Registry.ConverterRegistry.RegisterConverter(typeof(RRI), new DsonObjectConverter<RRI>(x => RadixConstants.StandardEncoding.GetBytes(x.ToString()), y => new RRI(RadixConstants.StandardEncoding.GetString(y))));
        //    //CborOptions.Default.Registry.ConverterRegistry.RegisterConverter(typeof(UInt384), new DsonObjectConverter<UInt384>());
        //    CborOptions.Default.Registry.ConverterRegistry.RegisterConverter(typeof(AID), new DsonObjectConverter<AID>(x => x.Bytes, y => new AID(y)));

        //    CborOptions.Default.Registry.ObjectMappingConventionRegistry.RegisterProvider(new OptInObjectMappingConventionProvider());
        //    CborOptions.Default.Registry.DefaultDiscriminatorConvention.RegisterAssembly(typeof(Particle).Assembly);
        //    //CborOptions.Default.Registry.ObjectMappingRegistry.Register<Particle>(om =>
        //    //{
        //    //    om.AutoMap();
        //    //    om.MapCreator(o => new );
        //    //});

        //    // initialize the default config for each output mode
        //    foreach (var mode in (OutputMode[])Enum.GetValues(typeof(OutputMode)))
        //        _outputModeOptions.Add(mode, CborOptions.Default);

        //    // register output mode mappings
        //    #region Hash

        //    _outputModeOptions[OutputMode.Hash].Registry.ObjectMappingRegistry.Register<Atom>(om =>
        //    {
        //        om.AutoMap();
        //        om.ClearMemberMappings();
        //        om.MapMember(o => o.ParticleGroups);
        //        om.MapMember(o => o.MetaData);
        //        om.MapMember(o => o.Id); // TODO Test if this is auto excluded because there is no attribute
        //    });
        //    //_outputModeOptions[OutputMode.Hash].Registry.ObjectMappingRegistry.Register<ECKeyPair>(om =>
        //    //{
        //    //    om.AutoMap();
        //    //    om.ClearMemberMappings();
        //    //    om.MapMember(o => o.PublicKeyBytes);
        //    //    om.MapMember(o => o.PrivateKeyBytes);
        //    //});

        //    #endregion

        //    #region Api

        //    //_outputModeOptions[OutputMode.Api].Registry.ObjectMappingRegistry.Register<ECKeyPair>(om =>
        //    //{
        //    //    om.AutoMap();
        //    //    om.ClearMemberMappings();
        //    //    om.MapMember(o => o.PublicKey);
        //    //});

        //    #endregion

        //    #region Wire

        //    //_outputModeOptions[OutputMode.Wire].Registry.ObjectMappingRegistry.Register<ECKeyPair>(om =>
        //    //{
        //    //    om.AutoMap();
        //    //    om.ClearMemberMappings();
        //    //    om.MapMember(o => o.PublicKey);
        //    //});

        //    #endregion

        //    #region Persist



        //    #endregion

        //}

        public static CborOptions GetDsonOptions(OutputMode mode = OutputMode.All)
        {
            var options = _outputModeOptions[mode];

            return options;
        }

        /// <summary>
        /// Used to add custom mappings for classes which reside above the serialization layer
        /// </summary>
        /// <param name="objectMapping">dictionary containing the object mappings per type</param>
        public static void AddDsonMappingRegistry(Dictionary<OutputMode, IObjectMapping> objectMapping)
        {
            foreach (var mode in (OutputMode[])Enum.GetValues(typeof(OutputMode)))
            {
                if (objectMapping[mode] == null || objectMapping.Count == 0)
                    continue;

                _outputModeOptions[mode].Registry.ObjectMappingRegistry.Register(objectMapping[mode]);
            }
        }

        // required for not serializing fields and properties by default
        //public class OptInObjectMappingConventionProvider : IObjectMappingConventionProvider
        //{
        //    public IObjectMappingConvention GetConvention(Type type)
        //    {
        //        // here you could filter which type should be optIn and return null for other types
        //        return new OptInObjectMappingConvention();
        //    }
        //}
        //public class OptInObjectMappingConvention : IObjectMappingConvention
        //{
        //    private readonly DefaultObjectMappingConvention _defaultConvention = new DefaultObjectMappingConvention();

        //    public void Apply<T>(SerializationRegistry registry, ObjectMapping<T> objectMapping) where T : class
        //    {
        //        _defaultConvention.Apply(registry, objectMapping);

        //        // restrict to members holding CborPropertyAttribute
        //        objectMapping.SetMemberMappings(objectMapping.MemberMappings
        //            .Where(m => m.MemberInfo.IsDefined(typeof(CborPropertyAttribute), true)).ToList());
        //    }
        //}
    }
}
