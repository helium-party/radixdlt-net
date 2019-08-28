using System.Linq;
using Dahomey.Cbor.Attributes;
using Newtonsoft.Json;

namespace HeliumParty.RadixDLT.EllipticCurve
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ECKeyPair
    {
        private const string Curvealgo = "secp256k1";
        private const string Keypairalgo = "ECDSA";

        [CborProperty("public"), JsonProperty(PropertyName = "public")]
        private byte[] _publicKey => PublicKey.Base64Array;

        [CborProperty("private"), JsonProperty(PropertyName = "private")]
        private byte[] _privateKey => PrivateKey.Base64Array;

        public ECPublicKey PublicKey { get; }
        public ECPrivateKey PrivateKey { get; }

        public ECKeyPair(ECPrivateKey privateKey, ECPublicKey publicKey)
        {            
            PublicKey = publicKey;
            PrivateKey = privateKey;
        }

        [CborConstructor, JsonConstructor]
        public ECKeyPair(byte[] privateKey, byte[] publicKey)
        {
            PublicKey = new ECPublicKey(publicKey);
            PrivateKey = new ECPrivateKey(privateKey);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ECKeyPair))
                return false;

            var other = (ECKeyPair)obj;
            return _publicKey.SequenceEqual(other._publicKey) && _privateKey.SequenceEqual(other._privateKey);
        }
    }
}