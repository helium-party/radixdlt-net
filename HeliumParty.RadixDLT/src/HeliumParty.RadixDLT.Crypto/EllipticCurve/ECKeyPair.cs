using Dahomey.Cbor.Attributes;
using Newtonsoft.Json;

namespace HeliumParty.RadixDLT.EllipticCurve
{
    public class ECKeyPair
    {
        private const string Curvealgo = "secp256k1";
        private const string Keypairalgo = "ECDSA";

        [CborProperty("public"), JsonProperty(PropertyName = "public")]
        public ECPublicKey PublicKey { get; }

        [CborProperty("private"), JsonProperty(PropertyName = "private")]
        public ECPrivateKey PrivateKey { get; }

        public ECKeyPair(ECPrivateKey privateKey, ECPublicKey publicKey)
        {
            PublicKey = publicKey;
            PrivateKey = privateKey;
        }

        public ECKeyPair(byte[] privateKey, byte[] publicKey)
        {
            PublicKey = new ECPublicKey(publicKey);
            PrivateKey = new ECPrivateKey(privateKey);
        }
    }
}