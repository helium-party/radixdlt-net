namespace HeliumParty.RadixDLT.EllipticCurve
{
    public class ECKeyPair
    {
        private const string Curvealgo = "secp256k1";
        private const string Keypairalgo = "ECDSA";

        public ECPublicKey PublicKey { get; }
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