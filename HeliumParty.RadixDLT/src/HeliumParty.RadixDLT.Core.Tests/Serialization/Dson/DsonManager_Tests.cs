﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Dahomey.Cbor.Attributes;
using HeliumParty.RadixDLT.Atoms;
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

namespace HeliumParty.RadixDLT.Core.Tests.Serialization.Dson
{
    public class DsonManager_Tests
    {
        private readonly DsonManager _manager;

        public DsonManager_Tests()
        {
            _manager = new DsonManager();
        }


        #region naming convention

        class IntObject
        {
            public int IntValue { get; set; }

            [CborProperty("Test.From")]
            public int IntSecondValue { get; set; }

            public string HiddenValue { get; set; }
        }

        [Fact]
        public void Dson_NamingConventions_Test()
        {
            //arrange
            var o = new IntObject() { IntValue = 300 , IntSecondValue = 20};

            //act
            var dson = _manager.ToDson(o);            
            var o2 = _manager.FromDson<IntObject>(dson, OutputMode.All);

            //assert
            o2.IntValue.ShouldBe(o.IntValue);
            o2.IntSecondValue.ShouldBe(o.IntSecondValue);
        }


        #endregion

        #region Base Types

        [Fact]
        public async Task Byte_Parsing_Test()
        {
            var bytes = Bytes.FromHexString("0123456789abcdef");
            var serializedBytes = await _manager.ToDsonAsync(bytes, OutputMode.All);
            var deserializedBytes = _manager.FromDson<byte[]>(serializedBytes, OutputMode.All);
            deserializedBytes.ShouldBe(bytes);
        }

        [Fact]
        public async Task EUID_Parsing_Test()
        {
            var euid = new EUID("1e340377ac58b9008ad12e1f2bae015d");
            var serializedEuid = await _manager.ToDsonAsync(euid, OutputMode.All);
            var deserializedEuid = _manager.FromDson<EUID>(serializedEuid, OutputMode.All);
            deserializedEuid.ShouldBe(euid);
        }

        // TODO add TestHash once implemented

        [Fact]
        public void TestRadixAddressDson()
        {
            var addr = new RadixAddress("17E8ZCLeczaBe4C6fJ3x649XWTPcmYukz6Bw18zFNgdxwhdukHc");
            var serializedAddr = _manager.ToDson(addr);
            var deserializedAddr = _manager.FromDson<RadixAddress>(serializedAddr, OutputMode.All);
            deserializedAddr.ShouldBe(addr);
        }

        //TODO implement TestUInt256 once implemented

        [Fact]
        public void TestRadixRRIDson()
        {
            var rri = new RRI(new RadixAddress("17E8ZCLeczaBe4C6fJ3x649XWTPcmYukz6Bw18zFNgdxwhdukHc"), "uniqueString");
            var serializedRri = _manager.ToDson(rri);
            var deserializedRri = _manager.FromDson<RRI>(serializedRri, OutputMode.All);
            deserializedRri.ShouldBe(rri);
        }

        // TODO add AID

        #endregion

        #region Crypto Layer

        [Fact]
        public void TestECSignatureDson()
        {
            var eCKeyManager = new ECKeyManager();
            var address = eCKeyManager.GetRandomKeyPair();
            var signature = eCKeyManager.GetECSignature(address.PrivateKey, Bytes.FromBase64String("testtest"));
            var serialized = _manager.ToDson(signature);

            var deserialized = _manager.FromDson<ECSignature>(serialized);
            deserialized.ShouldBe(signature);
        }

        [Fact]
        public void ECKeyPair_Parsing_Test()
        {
            var eCKeyManager = new ECKeyManager();
            var address = eCKeyManager.GetRandomKeyPair();
            var serialized = _manager.ToDson(address, OutputMode.Hash);

            var deserialized = _manager.FromDson<ECKeyPair>(serialized);
            deserialized.PublicKey.Base64.ShouldBe(address.PublicKey.Base64);
        }

        #endregion

        #region Core Layer



        [Fact]
        public void TestMessageParticleDson()
        {
            var eCKeyManager = new ECKeyManager();
            var address1 = new RadixAddress(10, eCKeyManager.GetRandomKeyPair().PublicKey);
            var address2 = new RadixAddress(10, eCKeyManager.GetRandomKeyPair().PublicKey);
            var messageParticle = new MessageParticle(address1, address2, new Dictionary<string, string> { { "key", "value" } }, Bytes.FromBase64String("testtest"), 0L, new HashSet<EUID>
            {
                address1.EUID, address2.EUID
            });
            var serialized = _manager.ToDson(messageParticle);
            var deserialized = _manager.FromDson<MessageParticle>(serialized);
        }

        [Fact]
        public void TestRRIParticleDson()
        {
            var eCKeyManager = new ECKeyManager();
            var address = new RadixAddress(10, eCKeyManager.GetRandomKeyPair().PublicKey);
            var rriParticle = new RRIParticle(new RRI(address, "test"));
            var serialized = _manager.ToDson(rriParticle);
            var deserialized = _manager.FromDson<RRIParticle>(serialized);
        }

        [Fact]
        public void TestUniqeParticleDson()
        {
            var eCKeyManager = new ECKeyManager();
            var address = new RadixAddress(10, eCKeyManager.GetRandomKeyPair().PublicKey);
            var uniqeParticle = new UniqueParticle(address, "test");
            var serialized = _manager.ToDson(uniqeParticle);
            var deserialized = _manager.FromDson<UniqueParticle>(serialized);
        }

        //[Fact]
        //public void TestSpunParticleDson()
        //{
        //    var eCKeyManager = new ECKeyManager();
        //    var address = new RadixAddress(10, eCKeyManager.GetRandomKeyPair().PublicKey);
        //    var rriParticle = new RRIParticle(new RRI(address, "test"));
        //    var spunParticle = new SpunParticle(rriParticle, Spin.Up);

        //    var serialized = _manager.ToDson(spunParticle);
        //    var x = Bytes.ToHexString(serialized);
        //    var deserialized = _manager.FromDson<SpunParticle>(serialized);
        //}

        //[Fact]
        //public void TestParticleGroupDson()
        //{
        //    var eCKeyManager = new ECKeyManager();
        //    var address = new RadixAddress(10, eCKeyManager.GetRandomKeyPair().PublicKey);
        //    var rriParticle = new RRIParticle(new RRI(address, "test"));
        //    var spunParticle = new SpunParticle(rriParticle, Spin.Up);
        //    // TODO implement this once TestSpunParticle is working
        //    //var particleList = new List<SpunParticle>();
        //    //var particleGroup = new ParticleGroup(new List<SpunParticle>{spunParticle, spunParticle}.ToImmutableList(), ImmutableDictionary<string, string>.Empty );

        //    var serialized = _manager.ToDson(spunParticle);
        //    var deserialized = _manager.FromDson<ParticleGroup>(serialized);
        //}

        // TODO add unit test for Atom once TestParticleGroup is working

        #endregion
    }

}