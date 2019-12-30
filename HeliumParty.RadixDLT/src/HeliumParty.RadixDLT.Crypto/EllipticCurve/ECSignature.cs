using HeliumParty.RadixDLT.Primitives;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities.Encoders;

namespace HeliumParty.RadixDLT.EllipticCurve
{
    public class ECSignature
    {
        //protected readonly byte[] _r;
        //protected readonly byte[] _s;

        public byte[] R { get; protected set; }
        public byte[] S { get; protected set; }

        public ECSignature()
        {

        }

        public ECSignature(BigInteger r, BigInteger s)
        {
            R = Bytes.TrimLeadingZeros(r.ToByteArray());
            S = Bytes.TrimLeadingZeros(s.ToByteArray());
        }

        public ECSignature(byte[] r, byte[] s)
        {
            R = Bytes.TrimLeadingZeros(r);
            S = Bytes.TrimLeadingZeros(s);
        }

        public string GetRBase64() => Base64.ToBase64String(R);

        // Set sign to positive to stop BigInteger interpreting high bit as sign
        public BigInteger RInt => new BigInteger(1, R);

        // Set sign to positive to stop BigInteger interpreting high bit as sign
        public BigInteger SInt => new BigInteger(1, S);

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