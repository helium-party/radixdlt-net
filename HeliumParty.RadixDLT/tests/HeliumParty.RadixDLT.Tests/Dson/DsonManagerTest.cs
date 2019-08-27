using System.Collections.Generic;
using System.Collections.Immutable;
using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.EllipticCurve;
using HeliumParty.RadixDLT.EllipticCurve.Managers;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Particles;
using HeliumParty.RadixDLT.Particles.Types;
using HeliumParty.RadixDLT.Primitives;
using Shouldly;
using Xunit;

namespace HeliumParty.RadixDLT.Tests.Dson
{
    public class DsonManagerTest
    {
        private readonly DsonManager _manager;

        public DsonManagerTest()
        {
            _manager = new DsonManager();
        }

        #region Base Types

        [Fact]
        public void TestByteDson()
        {
            var bytes = Bytes.FromHexString("0123456789abcdef");
            var serializedBytes = _manager.ToDson(bytes);
            var deserializedBytes = _manager.FromDson<byte[]>(serializedBytes);
            deserializedBytes.ShouldBe(bytes);
        }

        [Fact]
        public void TestEUIDDson()
        {
            var euid = new EUID("1e340377ac58b9008ad12e1f2bae015d");
            var serializedEuid = _manager.ToDson(euid);
            var deserializedEuid = _manager.FromDson<EUID>(serializedEuid);
            deserializedEuid.ShouldBe(euid);
        }

        // TODO add TestHash once implemented

        [Fact]
        public void TestRadixAddressDson()
        {
            var addr = new RadixAddress("17E8ZCLeczaBe4C6fJ3x649XWTPcmYukz6Bw18zFNgdxwhdukHc");
            var serializedAddr = _manager.ToDson(addr);
            var deserializedAddr = _manager.FromDson<RadixAddress>(serializedAddr);
            deserializedAddr.ShouldBe(addr);
        }

        //TODO implement TestUInt256 once implemented

        [Fact]
        public void TestRadixRRIDson()
        {
            var rri = new RRI(new RadixAddress("17E8ZCLeczaBe4C6fJ3x649XWTPcmYukz6Bw18zFNgdxwhdukHc"), "uniqueString");
            var serializedRri = _manager.ToDson(rri);
            var deserializedRri = _manager.FromDson<RRI>(serializedRri);
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
            var serialized = _manager.ToDson(signature);

            var deserialized = _manager.FromDson<ECSignature>(serialized);
            deserialized.ShouldBe(signature);
        }

        [Fact]
        public void TestECKEyPair()
        {
            var eCKeyManager = new ECKeyManager();
            var address = eCKeyManager.GetRandomKeyPair();
            var serialized = _manager.ToDson(address);

            var deserialized = _manager.FromDson<ECKeyPair>(serialized);
            deserialized.ShouldBe(address);
        }

        #endregion
    }
}