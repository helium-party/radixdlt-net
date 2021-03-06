﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Dahomey.Cbor.ObjectModel;
using HeliumParty.BaseTest;
using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.Core.Tests.Resources;
using HeliumParty.RadixDLT.EllipticCurve;
using HeliumParty.RadixDLT.EllipticCurve.Managers;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Particles;
using HeliumParty.RadixDLT.Particles.Types;
using HeliumParty.RadixDLT.Primitives;
using HeliumParty.RadixDLT.Serialization;
using HeliumParty.RadixDLT.Serialization.Dson;
using Shouldly;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using HeliumParty.RadixDLT.Identity.Managers;

namespace HeliumParty.RadixDLT.Core.Tests.Serialization.Dson
{
    public class DsonManager_Tests : HbIntegratedBaseTest
    {
        private readonly IDsonManager _dsonmanager;
        private readonly IEUIDManager _euidmanager;

        public DsonManager_Tests()
        {
            _dsonmanager = IocContainer.GetService<IDsonManager>();
            _euidmanager = IocContainer.GetService<IEUIDManager>();
        }

        #region mapping convention

        class TestClass
        {
            [SerializationOutput(OutputMode.All)]
            public int IntValue { get; set; }
            [SerializationOutput(OutputMode.Hash)]
            public int IntSecondValue { get; set; }
            [SerializationOutput(OutputMode.None)]
            public string HiddenValue { get; set; }
            public string DummyValue { get; set; }
            public byte[] DummyBin { get; set; }
        }

        [Fact]
        public void Dson_MappingConventions_Test()
        {
            //arrange
            var o = new TestClass() {
                IntValue = 300 , IntSecondValue = 20, HiddenValue="secret", DummyValue="known",
                DummyBin = Bytes.FromHexString("0123456789abcdef")
            };

            //act
            var dson = _dsonmanager.ToDson(o);            
            var o2 = _dsonmanager.FromDson<TestClass>(dson, OutputMode.All);

            //assert
            o2.IntValue.ShouldBe(o.IntValue);
            o2.IntSecondValue.ShouldBe(o.IntSecondValue);
            o2.HiddenValue.ShouldBeNull();
            o2.DummyValue.ShouldBe(o.DummyValue);
            o2.DummyBin.ShouldBe(Bytes.FromHexString("0123456789abcdef"));
        }

        #endregion

        #region Base Types

        [Fact]
        public void Byte_Parsing_Test()
        {
            var bytes = Bytes.FromHexString("0123456789abcdef");
            var serializedBytes = _dsonmanager.ToDson(bytes, OutputMode.All);
            var deserializedBytes = _dsonmanager.FromDson<byte[]>(serializedBytes, OutputMode.All);
            deserializedBytes.ShouldBe(bytes);
        }

        [Fact]
        public void EUID_Parsing_Test()
        {
            var euid = new EUID("1e340377ac58b9008ad12e1f2bae015d");
            var serializedEuid = _dsonmanager.ToDson(euid, OutputMode.All);
            var deserializedEuid = _dsonmanager.FromDson<EUID>(serializedEuid, OutputMode.All);
            deserializedEuid.ShouldBe(euid);
        }

        // TODO add TestHash once implemented

        [Fact]
        public void RadixAddress_Parsing_Test()
        {
            var addr = new RadixAddress("17E8ZCLeczaBe4C6fJ3x649XWTPcmYukz6Bw18zFNgdxwhdukHc");
            var serializedAddr = _dsonmanager.ToDson(addr);
            var deserializedAddr = _dsonmanager.FromDson<RadixAddress>(serializedAddr);
            deserializedAddr.ShouldBe(addr);
        }

        [Fact]
        public void UInt256_Parsing_Test()
        {
            var numb = (UInt256) new byte[]{0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10};

            var serialized = _dsonmanager.ToDson(numb);
            var deserialized = _dsonmanager.FromDson<UInt256>(serialized);

            deserialized.ShouldBe(numb);
        }

        [Fact]
        public void RRI_Parsing_Test()
        {
            var rri = new RRI(new RadixAddress("17E8ZCLeczaBe4C6fJ3x649XWTPcmYukz6Bw18zFNgdxwhdukHc"), "uniqueString");
            var serialized = _dsonmanager.ToDson(rri);
            var deserialized = _dsonmanager.FromDson<RRI>(serialized, OutputMode.All);
            deserialized.ShouldBe(rri);
        }

        [Fact]
        public void AID_Parsing_Test()
        {
            var aid = new AID(Bytes.FromHexString("b5778220e5fa6063208498148555fbc19ac44ef1742806cb56abab3870585f04"));
            var serialized = _dsonmanager.ToDson(aid);
            var deserialized = _dsonmanager.FromDson<AID>(serialized, OutputMode.All);
            deserialized.ShouldBe(aid);
        }

        #endregion

        #region Crypto Layer

        [Fact]
        public void ECSignature_Parsing_Test()
        {
            var eCKeyManager = new ECKeyManager();
            var pair = eCKeyManager.GetRandomKeyPair();
            var signature = eCKeyManager.GetECSignature(pair.PrivateKey, Bytes.FromBase64String("testtest"));
            var serialized = _dsonmanager.ToDson(signature);

            var deserialized = _dsonmanager.FromDson<ECSignature>(serialized);
            deserialized.R.ShouldBe(signature.R);
            deserialized.S.ShouldBe(signature.S);
        }

        [Fact]
        public void ECKeyPair_Parsing_Test()
        {
            var eCKeyManager = new ECKeyManager();
            var address = eCKeyManager.GetRandomKeyPair();
            var serialized = _dsonmanager.ToDson(address, OutputMode.Hash);

            var deserialized = _dsonmanager.FromDson<ECKeyPair>(serialized);
            deserialized.PublicKey.Base64.ShouldBe(address.PublicKey.Base64);
            deserialized.PrivateKey.ShouldBeNull();
        }

        #endregion

        #region Core Layer

        [Fact]
        public void MessageParticle_Parsing_Test()
        {
            //arrange
            var eCKeyManager = new ECKeyManager();
            var address1 = new RadixAddress(10, eCKeyManager.GetRandomKeyPair().PublicKey);
            var address2 = new RadixAddress(10, eCKeyManager.GetRandomKeyPair().PublicKey);
            var messageParticle = new MessageParticle(
                address1, 
                address2, 
                new Dictionary<string, string> { { "key", "value" } }, 
                Bytes.FromBase64String("testtest"), 30L, new HashSet<EUID>
                {
                    _euidmanager.GetEUID(address1),
                    _euidmanager.GetEUID(address2)
                });

            //act
            var serialized = _dsonmanager.ToDson<Particle>(messageParticle);

            var deserialized = (MessageParticle)_dsonmanager.FromDson<Particle>(serialized);

            //assert
            deserialized.From.ShouldBe(address1);
            deserialized.To.ShouldBe(address2);
            deserialized.Nonce.ShouldBe(messageParticle.Nonce);
            deserialized.MetaData.ShouldBe(messageParticle.MetaData);
        }

        [Fact]
        public void RRIParticle_Parsing_Test()
        {
            var eCKeyManager = new ECKeyManager();
            var address = new RadixAddress(10, eCKeyManager.GetRandomKeyPair().PublicKey);
            var rriParticle = new RRIParticle(new RRI(address, "test"), _euidmanager.GetEUID(address));
            var serialized = _dsonmanager.ToDson<Particle>(rriParticle);

            var deserialized = _dsonmanager.FromDson<Particle>(serialized);

            ((RRIParticle)deserialized).RRI.Address.ECPublicKey.Base64.ShouldBe(address.ECPublicKey.Base64);
        }

        [Fact]
        public void UniqueParticle_Parsing_Test()
        {
            //arrange
            var eCKeyManager = new ECKeyManager();
            var address = new RadixAddress(10, eCKeyManager.GetRandomKeyPair().PublicKey);
            var uniqeParticle = new UniqueParticle(address, "test", _euidmanager.GetEUID(address));

            //act
            var serialized = _dsonmanager.ToDson(uniqeParticle);
            var deserialized = (UniqueParticle)_dsonmanager.FromDson<Particle>(serialized);

            //assert
            deserialized.Address.ShouldBe(uniqeParticle.Address);
        }

        [Fact]
        public void ParticleList_Parsing_Test()
        {
            //arrange
            var particles = new List<Particle>();
            var eCKeyManager = new ECKeyManager();
            var address1 = new RadixAddress(10, eCKeyManager.GetRandomKeyPair().PublicKey);
            var address2 = new RadixAddress(10, eCKeyManager.GetRandomKeyPair().PublicKey);
            var messageParticle = new MessageParticle(address1, address2, new Dictionary<string, string> { { "key", "value" } }, Bytes.FromBase64String("testtest"), 0L, new HashSet<EUID>
            {
                    _euidmanager.GetEUID(address1),
                    _euidmanager.GetEUID(address2)
            });
            var address = new RadixAddress(10, eCKeyManager.GetRandomKeyPair().PublicKey);
            var uniqeParticle = new UniqueParticle(address, "test", _euidmanager.GetEUID(address));

            particles.Add(messageParticle);
            particles.Add(uniqeParticle);

            //act
            var serialized = _dsonmanager.ToDson(particles);
            var deserialized = _dsonmanager.FromDson<List<Particle>>(serialized);

            //assert
            deserialized.ShouldNotBeNull();
            deserialized.Count.ShouldBe(particles.Count);
        }

        [Fact]
        public void SpunParticle_Parsing_Test()
        {
            //arrange
            var eCKeyManager = new ECKeyManager();
            var address = new RadixAddress(10, eCKeyManager.GetRandomKeyPair().PublicKey);
            var rriParticle = new RRIParticle(new RRI(address, "test"), _euidmanager.GetEUID(address));
            var spunParticle = new SpunParticle(rriParticle, Spin.Up);

            //act
            var serialized = _dsonmanager.ToDson(spunParticle);
            var x = Bytes.ToHexString(serialized);
            var deserialized = _dsonmanager.FromDson<SpunParticle>(serialized);
            var cborobj = _dsonmanager.FromDson<CborObject>(serialized);

            //assert
            ((RRIParticle)deserialized.Particle).RRI.Name.ShouldBe(rriParticle.RRI.Name);
        }

        [Fact]
        public void MessageParticle_Deserializing_Test()
        {
            //arrange
            var data = ResourceParser.GetResource("messageParticle3.dson");

            //act
            var cbor = _dsonmanager.FromDson<CborObject>(data);
            var mp = (MessageParticle) _dsonmanager.FromDson<Particle>(data);            

            //assert
            mp.ShouldNotBeNull();
            mp.Nonce.ShouldBe(2181035975144481159);
            mp.From.ToString().ShouldBe("JEbhKQzBn4qJzWJFBbaPioA2GTeaQhuUjYWkanTE6N8VvvPpvM8");
            mp.To.ToString().ShouldBe("JEbhKQzBn4qJzWJFBbaPioA2GTeaQhuUjYWkanTE6N8VvvPpvM8");

        }

        [Fact]
        public void ParticleGroup_Parsing_Test()
        {
            //arrange
            var eCKeyManager = new ECKeyManager();
            var address1 = new RadixAddress(10, eCKeyManager.GetRandomKeyPair().PublicKey);
            var address2 = new RadixAddress(10, eCKeyManager.GetRandomKeyPair().PublicKey);
            var messageParticle = new MessageParticle(address1, address2, new Dictionary<string, string> { { "key", "value" } }, Bytes.FromBase64String("testtest"), 30L, new HashSet<EUID>
            {
                _euidmanager.GetEUID(address1),
                _euidmanager.GetEUID(address2)
            });

            var spunp = new SpunParticle(messageParticle, Spin.Down);
            var listbuilder = ImmutableList.CreateBuilder<SpunParticle>();
            listbuilder.Add(spunp);

            var mdatabuilder = ImmutableDictionary.CreateBuilder<string, string>();
            mdatabuilder.Add(new KeyValuePair<string, string>("Test", "Test"));

            var group =
                new ParticleGroup(listbuilder.ToImmutableList(), mdatabuilder.ToImmutableDictionary());

            //act
            var dson = _dsonmanager.ToDson(group, OutputMode.All);
            var deserialized = _dsonmanager.FromDson<ParticleGroup>(dson, OutputMode.All);

            //assert
            deserialized.ShouldNotBeNull();
            deserialized.Particles.Count.ShouldBe(1);
            deserialized.Particles.First().Particle.ShouldBeOfType<MessageParticle>();
        }

        [Fact]
        public void Atom_Deserializing_Test()
        {
            //arrange
            //create own atom
            var eCKeyManager = new ECKeyManager();
            var address1 = new RadixAddress(10, eCKeyManager.GetRandomKeyPair().PublicKey);
            var address2 = new RadixAddress(10, eCKeyManager.GetRandomKeyPair().PublicKey);
            var messageParticle = new MessageParticle(address1, address2, new Dictionary<string, string> { { "key", "value" } }, Bytes.FromBase64String("testtest"), 30L, new HashSet<EUID>
            {
                _euidmanager.GetEUID(address1),
                _euidmanager.GetEUID(address2)
            });

            var spunp = new SpunParticle(messageParticle, Spin.Down);
            var spunp2 = new SpunParticle(messageParticle, Spin.Down);
            var listbuilder = ImmutableList.CreateBuilder<SpunParticle>();
            listbuilder.Add(spunp);
            listbuilder.Add(spunp2);

            var mdatabuilder = ImmutableDictionary.CreateBuilder<string, string>();
            mdatabuilder.Add(new KeyValuePair<string, string>("Test", "Test"));

            var group = new ParticleGroup(listbuilder.ToImmutableList(), mdatabuilder.ToImmutableDictionary());

            var metaData = new SortedDictionary<string, string>();
            metaData.Add("work", "please");

            var cAtom = new Atom()
            {
                ParticleGroups = new List<ParticleGroup>() { group },
                MetaData = metaData
            };

            var data = ResourceParser.GetResource("messageatom.dson");

            //act
            var cdsonatom = _dsonmanager.ToDson(cAtom);

            var cboratom = _dsonmanager.FromDson<Atom>(data);
            var cborownatom = _dsonmanager.FromDson<Atom>(cdsonatom);
            //var cbor = CBORObject.DecodeFromBytes(data, CBOREncodeOptions.Default);


            //assert
            cboratom.ShouldNotBeNull();
            cborownatom.ShouldNotBeNull();
        }

        [Fact]
        public void Atom_Hash_Test()
        {
            var address1 = new RadixAddress(10, _dsonmanager.FromDson<ECKeyPair>(ResourceParser.GetResource("ECKeypair.dson")).PublicKey);
            var address2 = new RadixAddress(10, _dsonmanager.FromDson<ECKeyPair>(ResourceParser.GetResource("ECKeypair.dson")).PublicKey);

            var messageParticle = new MessageParticle(address1, address2, new Dictionary<string, string> { { "key", "value" } }, Bytes.FromBase64String("test"), 0, new HashSet<EUID>
            {
                _euidmanager.GetEUID(address1),
                _euidmanager.GetEUID(address2)
            });

            var spunParticle = new SpunParticle(messageParticle, Spin.Up);
            var spunParticle2 = new SpunParticle(messageParticle, Spin.Up);
            var listbuilder = ImmutableList.CreateBuilder<SpunParticle>();
            listbuilder.Add(spunParticle);

            var mdatabuilder = ImmutableDictionary.CreateBuilder<string, string>();

            var group = new ParticleGroup(listbuilder.ToImmutableList(), mdatabuilder.ToImmutableDictionary());

            var metaData = new Dictionary<string, string>();
            metaData.Add("b", "123");
            metaData.Add("a", "123");

            var atom = new Atom(group, 0L);

            // this hash has been produced with the java implementation
            atom.Hash.ToHexString().ShouldBe("33c09efcd8ea0d04823ebe5087f68b9ad586bb661be71a7a793f94c15fd699d3");
        }

        #endregion
    }
}
