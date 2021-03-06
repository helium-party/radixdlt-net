﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dahomey.Cbor;
using Dahomey.Cbor.Attributes;
using HeliumParty.DependencyInjection;
using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.EllipticCurve;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Primitives;
using Org.BouncyCastle.Math;

namespace HeliumParty.RadixDLT.Serialization.Dson
{
    public class DsonManager : IDsonManager , ITransientDependency
    {
        private readonly Dictionary<OutputMode, CborOptions> _outputModeOptions;

        public DsonManager()
        {
            _outputModeOptions = new Dictionary<OutputMode, CborOptions>();
            InitializeOptions();
        }

        public byte[] ToDson<T>(T obj, OutputMode mode = OutputMode.All) => ToDsonAsync(obj, mode).Result;

        public T FromDson<T>(byte[] bytes, OutputMode mode = OutputMode.All) => FromDsonAsync<T>(bytes, mode).Result;

        public virtual async Task<byte[]> ToDsonAsync<T>(T obj, OutputMode mode)
        {
            using (var ms = new MemoryStream())
            {
                var options = GetDsonOptions(mode);
                await Cbor.SerializeAsync(obj, ms, options);
                return ms.ToArray();
            }
        }

        public virtual async Task<T> FromDsonAsync<T>(byte[] bytes, OutputMode mode)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                ms.Write(bytes, 0, bytes.Length); // TODO modify this once .net standard 2.1 is used
                return await Cbor.DeserializeAsync<T>(ms, GetDsonOptions(mode));
            }
        }

        protected virtual CborOptions GetDsonOptions(OutputMode mode) => _outputModeOptions[mode];

        protected virtual void InitializeOptions()
        {
            foreach (var mode in (OutputMode[])Enum.GetValues(typeof(OutputMode)))
            {
                var options = new CborOptions();

                // specify how arrays and lists are serialized
                options.ArrayLengthMode = LengthMode.DefiniteLength;
                options.MapLengthMode = LengthMode.IndefiniteLength;

                // register converters
                options.Registry.ConverterRegistry.RegisterConverter(typeof(byte[]), new DsonObjectConverter<byte[]>(x => x, y => y));
                options.Registry.ConverterRegistry.RegisterConverter(typeof(UInt256), new DsonObjectConverter<UInt256>(x => x, y => y)); //implicit conversion
                options.Registry.ConverterRegistry.RegisterConverter(typeof(EUID), new DsonObjectConverter<EUID>(x => x.ToByteArray(), y => new EUID(y)));
                options.Registry.ConverterRegistry.RegisterConverter(typeof(ECPrivateKey), new DsonObjectConverter<ECPrivateKey>(x => x.Base64Array, y => new ECPrivateKey(y)));
                options.Registry.ConverterRegistry.RegisterConverter(typeof(ECPublicKey), new DsonObjectConverter<ECPublicKey>(x => x.Base64Array, y => new ECPublicKey(y)));
                options.Registry.ConverterRegistry.RegisterConverter(typeof(BigInteger), new DsonObjectConverter<BigInteger>(x => x.ToByteArray(), y => new BigInteger(1, y)));
                options.Registry.ConverterRegistry.RegisterConverter(typeof(RadixAddress), new DsonObjectConverter<RadixAddress>(x => Base58Encoding.Decode(x.ToString()), y => new RadixAddress(Base58Encoding.Encode(y))));
                options.Registry.ConverterRegistry.RegisterConverter(typeof(RRI), new DsonObjectConverter<RRI>(x => RadixConstants.StandardEncoding.GetBytes(x.ToString()), y => new RRI(RadixConstants.StandardEncoding.GetString(y))));
                options.Registry.ConverterRegistry.RegisterConverter(typeof(AID), new DsonObjectConverter<AID>(x => x.Bytes, y => new AID(y)));

                // register the custom ObjectMappingProvider
                options.Registry.ObjectMappingConventionRegistry.RegisterProvider(new DsonObjectMappingConventionProvider(mode));

                // manually register foreign classes 
                // ECKeypair
                // For security reasons the private Key is only serialized on PERSIST
                if (mode == OutputMode.Persist)
                {
                    options.Registry.ObjectMappingRegistry.Register<ECKeyPair>(om =>
                    {
                        om.AutoMap();
                        om.ClearMemberMappings();
                        om.MapMember(m => m.PrivateKey).SetMemberName("private");
                        om.MapMember(m => m.PublicKey).SetMemberName("public");
                        om.SetDiscriminator("crypto.ec_key_pair").SetDiscriminatorPolicy(CborDiscriminatorPolicy.Always);
                    });
                }
                else
                {
                    options.Registry.ObjectMappingRegistry.Register<ECKeyPair>(om =>
                    {
                        om.AutoMap();
                        om.ClearMemberMappings();
                        om.MapMember(m => m.PublicKey).SetMemberName("public");
                        om.SetDiscriminator("crypto.ec_key_pair").SetDiscriminatorPolicy(CborDiscriminatorPolicy.Always);
                    });
                }

                // ECSignature
                options.Registry.ObjectMappingRegistry.Register<ECSignature>(om =>
                {
                    om.AutoMap();
                    om.ClearMemberMappings();
                    om.MapMember(m => m.R);
                    om.MapMember(m => m.S);
                    om.SetDiscriminator("crypto.ecdsa_signature").SetDiscriminatorPolicy(CborDiscriminatorPolicy.Always);
                });

                // register the discriminator
                var discriminator = new DsonDiscriminator(options.Registry);
                options.Registry.DiscriminatorConventionRegistry.ClearConventions();
                options.Registry.DiscriminatorConventionRegistry.RegisterConvention(discriminator);

                var assemblies = new List<Assembly>
                {
                    // Assemblys which shall be searched for a CborDiscriminator Attribute
                    Assembly.GetAssembly(typeof(Atom)),
                };
                foreach (var type in assemblies.SelectMany(assembly => assembly.GetTypes().Where(t => Attribute.IsDefined(t, typeof(CborDiscriminatorAttribute)))))
                {
                    options.Registry.DiscriminatorConventionRegistry.RegisterType(type);
                }

                _outputModeOptions.Add(mode, options);
            }
        }
    }
}
