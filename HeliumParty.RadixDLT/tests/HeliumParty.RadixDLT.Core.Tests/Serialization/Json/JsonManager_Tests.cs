using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using HeliumParty.BaseTest;
using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.EllipticCurve;
using HeliumParty.RadixDLT.EllipticCurve.Managers;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Identity.Managers;
using HeliumParty.RadixDLT.Particles;
using HeliumParty.RadixDLT.Particles.Types;
using HeliumParty.RadixDLT.Primitives;
using HeliumParty.RadixDLT.Serialization;
using HeliumParty.RadixDLT.Serialization.Json;
using Shouldly;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace HeliumParty.RadixDLT.Core.Tests.Serialization.Json
{
    public class JsonManager_Tests : HbIntegratedBaseTest
    {
        private readonly IJsonManager _jsonmanager;
        private readonly IEUIDManager _euidmanager;

        public JsonManager_Tests()
        {
            _jsonmanager = IocContainer.GetService<IJsonManager>();
            _euidmanager = IocContainer.GetService<IEUIDManager>();
        }

        #region Base Types

        [Fact]
        public void ToByteJson()
        {
            var bytes = Bytes.FromHexString("0123456789abcdef");
            var serializedBytes = _jsonmanager.ToJson(bytes, OutputMode.All);
            var deserializedBytes = _jsonmanager.FromJson<byte[]>(serializedBytes, OutputMode.All);
            deserializedBytes.ShouldBe(bytes);
        }

        [Fact]
        public void TestEUIDJson()
        {
            var euid = new EUID("1e340377ac58b9008ad12e1f2bae015d");
            var serializedEuid = _jsonmanager.ToJson(euid, OutputMode.All);
            var deserializedEuid = _jsonmanager.FromJson<EUID>(serializedEuid, OutputMode.All);
            deserializedEuid.ShouldBe(euid);
        }

        // TODO add TestHash once implemented

        [Fact]
        public void TestRadixAddressJson()
        {
            var addr = new RadixAddress("17E8ZCLeczaBe4C6fJ3x649XWTPcmYukz6Bw18zFNgdxwhdukHc");
            var serializedAddr = _jsonmanager.ToJson(addr, OutputMode.All);
            var deserializedAddr = _jsonmanager.FromJson<RadixAddress>(serializedAddr, OutputMode.All);
            deserializedAddr.ShouldBe(addr);
        }

        [Fact]
        public void TestUInt256Json()
        {
            var numb = (UInt256)new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10 };
            var serialized = _jsonmanager.ToJson(numb);
            var deserialized = _jsonmanager.FromJson<UInt256>(serialized);
            deserialized.ShouldBe(numb);
        }

        [Fact]
        public void TestRadixRRIJson()
        {
            var rri = new RRI(new RadixAddress("17E8ZCLeczaBe4C6fJ3x649XWTPcmYukz6Bw18zFNgdxwhdukHc"), "uniqueString");
            var serializedRri = _jsonmanager.ToJson(rri, OutputMode.All);
            var deserializedRri = _jsonmanager.FromJson<RRI>(serializedRri, OutputMode.All);
            deserializedRri.ShouldBe(rri);
        }

        // TODO add AID

        #endregion

        #region Crypto Layer

        [Fact]
        public void TestECSignatureJson()
        {
            var eCKeyManager = new ECKeyManager();
            var address = eCKeyManager.GetRandomKeyPair();
            var signature = eCKeyManager.GetECSignature(address.PrivateKey, Bytes.FromBase64String("testtest"));
            var serialized = _jsonmanager.ToJson(signature);

            var deserialized = _jsonmanager.FromJson<ECSignature>(serialized);
            deserialized.R.ShouldBe(signature.R);
            deserialized.S.ShouldBe(signature.S);
        }

        [Fact]
        public void TestECKeyPairJson()
        {
            var eCKeyManager = new ECKeyManager();
            var address = eCKeyManager.GetRandomKeyPair();
            var serialized = _jsonmanager.ToJson(address, OutputMode.Hash);

            var deserialized = _jsonmanager.FromJson<ECKeyPair>(serialized);
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
            var serialized = _jsonmanager.ToJson<Particle>(messageParticle);
            var deserialized = (MessageParticle)_jsonmanager.FromJson<Particle>(serialized);

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
            var serialized = _jsonmanager.ToJson<Particle>(rriParticle);
            var deserialized = _jsonmanager.FromJson<Particle>(serialized);

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
            var serialized = _jsonmanager.ToJson(uniqeParticle);
            var deserialized = _jsonmanager.FromJson<UniqueParticle>(serialized);

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
            var serialized = _jsonmanager.ToJson(particles);
            var deserialized = _jsonmanager.FromJson<List<Particle>>(serialized);

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
            var serialized = _jsonmanager.ToJson(spunParticle);
            var deserialized = _jsonmanager.FromJson<SpunParticle>(serialized);

            //assert
            ((RRIParticle)deserialized.Particle).RRI.Name.ShouldBe(rriParticle.RRI.Name);
        }

        [Fact]
        public void MessageParticle_Deserializing_Test()
        {
            //arrange
            var data = "{\"serializer\":\"radix.particles.message\",\"bytes\":\":byt:RXhhbXBsZSBtZXNzYWdl\"," +
                       "\"destinations\":[\":uid:5f3b51bb456c7f5202c3489a798bec53\",\":uid:fa2eee711e1854d045622cd3c012e13d\"]," +
                       "\"from\":\":adr:JEWDNQAYJxwe27JrHSStupxatvaxrcrrR6hwzrAbLmDH7BgEFeZ\",\"metaData\":{\"application\":" +
                       "\":str:test-app-id\"},\"nonce\":10,\"to\":\":adr:JG2uTCDwMUw51Ph8FMHxzz65AuPC1saTKpKMzqwJmQGEdYkGEXw\",\"version\":100}";

            //act
            var mp = (MessageParticle)_jsonmanager.FromJson<Particle>(data);

            //assert
            mp.ShouldNotBeNull();
            mp.Nonce.ShouldBe(10);
            mp.From.ToString().ShouldBe("JEWDNQAYJxwe27JrHSStupxatvaxrcrrR6hwzrAbLmDH7BgEFeZ");
            mp.To.ToString().ShouldBe("JG2uTCDwMUw51Ph8FMHxzz65AuPC1saTKpKMzqwJmQGEdYkGEXw");

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

            var group = new ParticleGroup(listbuilder.ToImmutableList(), mdatabuilder.ToImmutableDictionary());

            //act
            var json = _jsonmanager.ToJson(group, OutputMode.All);
            var deserialized = _jsonmanager.FromJson<ParticleGroup>(json, OutputMode.All);

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
            var groups = new List<ParticleGroup> {group};

            var metaData = new Dictionary<string, string>();
            metaData.Add("work", "please");
            metaData.Add(Atom.MetadataTimestampKey, 0L.ToString());

            var atom = new Atom(groups, metaData);

            var data = "{\"serializer\":\"radix.particles.message\",\"bytes\":\":byt:RXhhbXBsZSBtZXNzYWdl\"," +
                       "\"destinations\":[\":uid:5f3b51bb456c7f5202c3489a798bec53\",\":uid:fa2eee711e1854d045622cd3c012e13d\"]," +
                       "\"from\":\":adr:JEWDNQAYJxwe27JrHSStupxatvaxrcrrR6hwzrAbLmDH7BgEFeZ\",\"metaData\":{\"application\":" +
                       "\":str:test-app-id\"},\"nonce\":1566833127947,\"to\":\":adr:JG2uTCDwMUw51Ph8FMHxzz65AuPC1saTKpKMzqwJmQGEdYkGEXw\",\"version\":100}";

            //act
            var serialized = _jsonmanager.ToJson(atom);
            var deserialized = _jsonmanager.FromJson<Atom>(serialized);

            //assert
            bool x = deserialized.Hash == atom.Hash;
            deserialized.Hash.ShouldBe(atom.Hash);
        }

        #endregion
    }
}