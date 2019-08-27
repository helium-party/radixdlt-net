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
        private readonly byte[] _publicKey;

        [CborProperty("private"), JsonProperty(PropertyName = "private")]
        private readonly byte[] _privateKey;

        public byte[] PublicKeyBytes => PublicKey.Base64Array;
        public byte[] PrivateKeyBytes => PrivateKey.Base64Array;

        public ECPublicKey PublicKey => new ECPublicKey(_publicKey);
        public ECPrivateKey PrivateKey => new ECPrivateKey(_privateKey);

        public ECKeyPair(ECPrivateKey privateKey, ECPublicKey publicKey)
        {
            _publicKey = publicKey.Base64Array;
            _privateKey = privateKey.Base64Array;
        }

        [CborConstructor, JsonConstructor]
        public ECKeyPair(byte[] privateKey, byte[] publicKey)
        {
            _publicKey = publicKey;
            _privateKey = privateKey;
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