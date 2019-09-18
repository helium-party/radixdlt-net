﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Dahomey.Cbor;
using Dahomey.Cbor.Attributes;
using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.EllipticCurve;
using HeliumParty.RadixDLT.Identity;
using Org.BouncyCastle.Math;

namespace HeliumParty.RadixDLT.Serialization.Dson
{
    public class DsonManager
    {
        private readonly Dictionary<OutputMode, CborOptions> _outputModeOptions;// = new Dictionary<OutputMode, CborOptions>();
        public byte[] ToDson<T>(T obj, OutputMode mode = OutputMode.All) => ToDsonAsync(obj, mode).Result;        

        public T FromDson<T>(byte[] bytes, OutputMode mode = OutputMode.All) => FromDsonAsync<T>(bytes, mode).Result;

        public DsonManager()
        {
            _outputModeOptions = new Dictionary<OutputMode, CborOptions>();
            InitializeOptions();
        }

        public virtual async Task<byte[]> ToDsonAsync<T>(T obj, OutputMode mode)
        {
            using (var ms = new MemoryStream())
            {
                //var t = DsonOutputMapping.GetDsonOptions();
                var options = GetDsonOptions(mode);
                await Cbor.SerializeAsync(obj, ms, options);
                return ms.ToArray();                
            }
        }

        public virtual async Task<T> FromDsonAsync<T>(byte[] bytes, OutputMode mode = OutputMode.All)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                ms.Write(bytes, 0, bytes.Length); // TODO modify this once .net standard 2.1 is used
                return await Cbor.DeserializeAsync<T>(ms, GetDsonOptions(mode));
            }
        }

        protected virtual CborOptions GetDsonOptions(OutputMode mode)
        {
            var options = _outputModeOptions[mode];

            return options;
        }

        protected virtual void InitializeOptions()
        {
            foreach (var mode in (OutputMode[])Enum.GetValues(typeof(OutputMode)))
            {
                var discriminator = new DsonDiscriminator();
                discriminator.RegisterAssembly(Assembly.GetAssembly(typeof(Atom)));
                discriminator.RegisterType(typeof(ECSignature), RadixConstants.StandardEncoding.GetBytes("crypto.ecdsa_signature"));

                var options = new CborOptions();
                options.Registry.ObjectMappingConventionRegistry.RegisterProvider(new DsonObjectMappingConventionProvider(mode));
                options.DiscriminatorConvention = discriminator;
                options.DiscriminatorPolicy = CborDiscriminatorPolicy.Always;

                options.Registry.ConverterRegistry.RegisterConverter(typeof(byte[]), new DsonObjectConverter<byte[]>(x => x, y => y));
                //options.Registry.ConverterRegistry.RegisterConverter(typeof(UInt256), new DsonObjectConverter<byte[]>(x => x, y => y));
                options.Registry.ConverterRegistry.RegisterConverter(typeof(EUID), new DsonObjectConverter<EUID>(x => x.ToByteArray(), y => new EUID(y)));
                options.Registry.ConverterRegistry.RegisterConverter(typeof(ECPrivateKey), new DsonObjectConverter<ECPrivateKey>(x => x.Base64Array, y => new ECPrivateKey(y)));
                options.Registry.ConverterRegistry.RegisterConverter(typeof(ECPublicKey), new DsonObjectConverter<ECPublicKey>(x => x.Base64Array, y => new ECPublicKey(y)));
                options.Registry.ConverterRegistry.RegisterConverter(typeof(BigInteger), new DsonObjectConverter<BigInteger>(x => x.ToByteArray(), y => new BigInteger(1, y)));
                options.Registry.ConverterRegistry.RegisterConverter(typeof(RadixAddress), new DsonObjectConverter<RadixAddress>(x => Base58Encoding.Decode(x.ToString()), y => new RadixAddress(Base58Encoding.Encode(y))));
                options.Registry.ConverterRegistry.RegisterConverter(typeof(RRI), new DsonObjectConverter<RRI>(x => RadixConstants.StandardEncoding.GetBytes(x.ToString()), y => new RRI(RadixConstants.StandardEncoding.GetString(y))));
                options.Registry.ConverterRegistry.RegisterConverter(typeof(AID), new DsonObjectConverter<AID>(x => x.Bytes, y => new AID(y)));


                //hide the private key
                options.Registry.ObjectMappingRegistry.Register<ECKeyPair>(om =>
                {
                    om.ClearMemberMappings();
                    om.MapMember(m => m.PublicKey);
                });

                options.Registry.ObjectMappingRegistry.Register<ECSignature>(om =>
                {
                    om.AutoMap().SetDiscriminator("crypto.ecdsa_signature").SetDiscriminatorPolicy(CborDiscriminatorPolicy.Always);
                });




                _outputModeOptions.Add(mode, options);
            }
        }
    }
}
