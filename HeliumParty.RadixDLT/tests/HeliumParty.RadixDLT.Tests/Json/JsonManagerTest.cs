using System.Collections.Generic;
using HeliumParty.RadixDLT.EllipticCurve;
using HeliumParty.RadixDLT.EllipticCurve.Managers;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Particles.Types;
using HeliumParty.RadixDLT.Primitives;
using Shouldly;
using Xunit;

namespace HeliumParty.RadixDLT.Tests.Json
{
    public class JsonManagerTest
    {
        private readonly JsonManager _manager;

        public JsonManagerTest()
        {
            _manager = new JsonManager();
        }

        #region Base Types

        [Fact]
        public void ToByteJson()
        {
            var bytes = Bytes.FromHexString("0123456789abcdef");
            var serializedBytes = _manager.ToJson(bytes);
            var deserializedBytes = _manager.FromJson<byte[]>(serializedBytes);
            deserializedBytes.ShouldBe(bytes);
        }

        [Fact]
        public void TestEUIDJson()
        {
            var euid = new EUID("1e340377ac58b9008ad12e1f2bae015d");
            var serializedEuid = _manager.ToJson(euid);
            var deserializedEuid = _manager.FromJson<EUID>(serializedEuid);
            deserializedEuid.ShouldBe(euid);
        }

        // TODO add TestHash once implemented

        [Fact]
        public void TestRadixAddressJson()
        {
            var addr = new RadixAddress("17E8ZCLeczaBe4C6fJ3x649XWTPcmYukz6Bw18zFNgdxwhdukHc");
            var serializedAddr = _manager.ToJson(addr);
            var deserializedAddr = _manager.FromJson<RadixAddress>(serializedAddr);
            deserializedAddr.ShouldBe(addr);
        }

        //TODO implement TestUInt256 once implemented

        [Fact]
        public void TestRadixRRIJson()
        {
            var rri = new RRI(new RadixAddress("17E8ZCLeczaBe4C6fJ3x649XWTPcmYukz6Bw18zFNgdxwhdukHc"), "uniqueString");
            var serializedRri = _manager.ToJson(rri);
            var deserializedRri = _manager.FromJson<RRI>(serializedRri);
            deserializedRri.ShouldBe(rri);
        }

        // TODO add AID

        #endregion

        #region Crypto Layer

        [Fact]
        public void TestECSignature()
        {
            var eCKeyManager = new ECKeyManager();
            var address = eCKeyManager.GetRandomKeyPair();
            var signature = eCKeyManager.GetECSignature(address.PrivateKey, Bytes.FromBase64String("testtest"));
            var serialized = _manager.ToJson(signature);

            var deserialized = _manager.FromJson<ECSignature>(serialized);
            deserialized.ShouldBe(signature);
        }

        [Fact]
        public void TestECKEyPair()
        {
            var eCKeyManager = new ECKeyManager();
            var address = eCKeyManager.GetRandomKeyPair();
            var serialized = _manager.ToJson(address);

            var deserialized = _manager.FromJson<ECKeyPair>(serialized);
            deserialized.ShouldBe(address);
        }

        #endregion

        #region Core Layer

        //[Fact]
        public void TestFixedSupplyTokenDefinitionParticle()
        {
            //TODO implement once UInt256 is working
            //var eCKeyManager = new ECKeyManager();
            //var address = new RadixAddress(10, eCKeyManager.GetRandomKeyPair().PublicKey);
            //var messageParticle = new FixedSupplyTokenDefinitionParticle(new RRI(address, "test"), "test", "test_description", );
            //var serialized = _manager.ToJson(messageParticle);
            //var deserialized = _manager.FromJson<MessageParticle>(serialized);
        }

        //[Fact]
        public void TestMutableSupplyTokenDefinitionParticle()
        {
            //TODO implement once UInt256 is working
        }

        [Fact]
        public void TestMessageParticle()
        {
            var eCKeyManager = new ECKeyManager();
            var address1 = new RadixAddress(10, eCKeyManager.GetRandomKeyPair().PublicKey);
            var address2 = new RadixAddress(10, eCKeyManager.GetRandomKeyPair().PublicKey);
            var messageParticle = new MessageParticle(address1, address2, new Dictionary<string, string> { { "key", "value" } }, Bytes.FromBase64String("testtest"));
            var serialized = _manager.ToJson(messageParticle);
            var deserialized = _manager.FromJson<MessageParticle>(serialized);
        }

        [Fact]
        public void TestRRIParticle()
        {
            var eCKeyManager = new ECKeyManager();
            var address = new RadixAddress(10, eCKeyManager.GetRandomKeyPair().PublicKey);
            var messageParticle = new RRIParticle(new RRI(address, "test"));
            var serialized = _manager.ToJson(messageParticle);
            var deserialized = _manager.FromJson<RRIParticle>(serialized);
        }

        //[Fact]
        public void TestTransferableTokensParticle()
        {
            //TODO implement once UInt256 is working
        }

        //[Fact]
        public void TestUnallocatedTokensParticle()
        {
            //TODO implement once UInt256 is working
        }

        [Fact]
        public void TestUniqeParticle()
        {
            var eCKeyManager = new ECKeyManager();
            var address = new RadixAddress(10, eCKeyManager.GetRandomKeyPair().PublicKey);
            var messageParticle = new UniqueParticle(address, "test");
            var serialized = _manager.ToJson(messageParticle);
            var deserialized = _manager.FromJson<UniqueParticle>(serialized);
        }

        #endregion
    }
}