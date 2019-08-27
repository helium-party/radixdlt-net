using System.Linq;
using Dahomey.Cbor.Attributes;
using HeliumParty.RadixDLT.Primitives;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities.Encoders;

namespace HeliumParty.RadixDLT.EllipticCurve
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ECSignature
    {
        [CborProperty("r"), JsonProperty(PropertyName = "r")]
        private readonly byte[] _r;

        [CborProperty("s"), JsonProperty(PropertyName = "s")]
        private readonly byte[] _s;

        [CborConstructor, JsonConstructor]
        public ECSignature(byte[] r, byte[] s)
        {
            _r = Bytes.TrimLeadingZeros(r);
            _s = Bytes.TrimLeadingZeros(s);
        }

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

        public override bool Equals(object obj)
        {
            if (!(obj is ECSignature))
                return false;

            var other = (ECSignature)obj;
            return _r.SequenceEqual(other._r) && _s.SequenceEqual(other._s);
        }
    }
}