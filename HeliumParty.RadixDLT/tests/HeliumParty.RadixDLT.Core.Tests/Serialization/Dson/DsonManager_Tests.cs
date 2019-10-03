using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
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
            [SerializationOutput(OutputMode.Never)]
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
        public async Task Byte_Parsing_Test()
        {
            var bytes = Bytes.FromHexString("0123456789abcdef");
            var serializedBytes = await _dsonmanager.ToDsonAsync(bytes, OutputMode.All);
            var deserializedBytes = _dsonmanager.FromDson<byte[]>(serializedBytes, OutputMode.All);
            deserializedBytes.ShouldBe(bytes);
        }

        [Fact]
        public async Task EUID_Parsing_Test()
        {
            var euid = new EUID("1e340377ac58b9008ad12e1f2bae015d");
            var serializedEuid = await _dsonmanager.ToDsonAsync(euid, OutputMode.All);
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

        //[Fact]
        //public void UInt256_Parsing_Test()
        //{
        //    var numb = new UInt256();
        //    numb.s0 = 1000;

        //    var parsed = _manager.ToDson(numb);
        //    var numb2 = _manager.FromDson<UInt256>(parsed);
        //    UInt256 x = new BigInteger();
        //    var refrnc = _manager.FromDson<CborObject>(parsed);

        //    refrnc.ShouldNotBeNull();
        //}

        [Fact]
        public void AID_Parsing_Test()
        {
            var rri = new RRI(new RadixAddress("17E8ZCLeczaBe4C6fJ3x649XWTPcmYukz6Bw18zFNgdxwhdukHc"), "uniqueString");
            var serializedRri = _dsonmanager.ToDson(rri);
            var deserializedRri = _dsonmanager.FromDson<RRI>(serializedRri, OutputMode.All);
            deserializedRri.ShouldBe(rri);
        }

        // TODO add AID

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
            var deserialized = _dsonmanager.FromDson<UniqueParticle>(serialized);

            //assert
            deserialized.Address.ShouldBe(uniqeParticle.Address);
        }

        [Fact]
        public void ParticleList_Parsing_Test()
        {
            //arrange
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
        public async Task MessageParticle_Deserializing_Test()
        {
            //arrange
            var data = ResourceParser.GetResource("messageParticle3.dson");

            //act
            var cbor = await _dsonmanager.FromDsonAsync<CborObject>(data);
            var mp = (MessageParticle)await _dsonmanager.FromDsonAsync<Particle>(data);            

            //assert
            mp.ShouldNotBeNull();
            mp.Nonce.ShouldBe(2181035975144481159);
            mp.From.ToString().ShouldBe("JEbhKQzBn4qJzWJFBbaPioA2GTeaQhuUjYWkanTE6N8VvvPpvM8");
            mp.To.ToString().ShouldBe("JEbhKQzBn4qJzWJFBbaPioA2GTeaQhuUjYWkanTE6N8VvvPpvM8");

        }

        [Fact]
        public async Task ParticleGroup_Parsing_Test()
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
            var dson = await _dsonmanager.ToDsonAsync(group, OutputMode.All);
            var deserialized = await _dsonmanager.FromDsonAsync<ParticleGroup>(dson, OutputMode.All);

            //assert
            deserialized.ShouldNotBeNull();
            deserialized.Particles.Count.ShouldBe(1);
            deserialized.Particles.First().Particle.ShouldBeOfType<MessageParticle>();
        }

        [Fact]
        public async Task Atom_Deserializing_Test()
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

            var group =
                new ParticleGroup(listbuilder.ToImmutableList(), mdatabuilder.ToImmutableDictionary());




            var metaData = new Dictionary<string, string>();
            metaData.Add("work", "please");

            var cAtom = new Atom()
            {
                ParticleGroups = new List<ParticleGroup>() { group },
                MetaData = metaData
            };

            var data = ResourceParser.GetResource("messageatom.dson");

            //act
            var cdsonatom = _dsonmanager.ToDson(cAtom);

            var cboratom = await _dsonmanager.FromDsonAsync<Atom>(data);
            var cborownatom = await _dsonmanager.FromDsonAsync<Atom>(cdsonatom);
            //var cbor = CBORObject.DecodeFromBytes(data, CBOREncodeOptions.Default);


            //assert
            cboratom.ShouldNotBeNull();
            cborownatom.ShouldNotBeNull();
        }
        #endregion
    }

}
