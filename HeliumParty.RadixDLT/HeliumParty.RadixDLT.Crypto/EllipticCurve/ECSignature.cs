using HeliumParty.RadixDLT.Primitives;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities.Encoders;

namespace HeliumParty.RadixDLT.EllipticCurve
{
    public class ECSignature
    {
        private readonly byte[] _r;
        private readonly byte[] _s;

        public ECSignature(BigInteger r, BigInteger s)
        {
            _r = Bytes.TrimLeadingZeros(r.ToByteArray());
            _s = Bytes.TrimLeadingZeros(s.ToByteArray());
        }

        public string GetRBase64() => Base64.ToBase64String(_r);

        // Set sign to positive to stop BigInteger interpreting high bit as sign
        public BigInteger R => new BigInteger(1, _r);

        // Set sign to positive to stop BigInteger interpreting high bit as sign
        public BigInteger S => new BigInteger(1, _s);

        public static ECSignature DecodeFromDER(byte[] bytes)
        {
            DerSequence seq;
            byte[] r, s;

            using (var decoder = new Asn1InputStream(bytes))
            {
                seq = (DerSequence)decoder.ReadObject();
                r = seq[0].ToAsn1Object().GetEncoded();
                s = seq[1].ToAsn1Object().GetEncoded();
            };

            return new ECSignature(new BigInteger(1, r), new BigInteger(1, s));
        }
    }
}