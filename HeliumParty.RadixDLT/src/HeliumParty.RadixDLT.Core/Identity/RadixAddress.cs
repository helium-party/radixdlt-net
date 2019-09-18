using System;
using System.Collections.Generic;
using HeliumParty.RadixDLT.EllipticCurve;
using HeliumParty.RadixDLT.Hashing;
using HeliumParty.RadixDLT.Identity.Managers;
using HeliumParty.RadixDLT.Serialization;

namespace HeliumParty.RadixDLT.Identity
{
    [SerializationPrefix(Dson = 0x04, Json = "adr")]
    public class RadixAddress
    {
        private readonly string _addressBase58;        

        private EUID _euid;
        public EUID EUID
        {
            get
            {
                if (_euid == null)
                    _euid = new EUIDManager().GetEUID(this);
                return _euid;
            }
        }
        public virtual ECPublicKey ECPublicKey { get; } // TODO : should this be converted to an auto property?

        /// <summary>
        ///     Create a RadixAddres from a base58 string
        /// </summary>
        /// <param name="addressBase58"></param>
        public RadixAddress(string addressBase58)
        {
            byte[] raw = Base58Encoding.Decode(addressBase58);
            RadixHash check = RadixHash.From(raw, 0, raw.Length - 4);

            for (int i = 0; i < 4; ++i)
            {
                if (check[i] != raw[raw.Length - 4 + i])
                {
                    throw new ArgumentException("Address " + addressBase58 + " checksum mismatch");
                }
            }

            byte[] publicKey = new byte[raw.Length - 5];
            Array.Copy(raw, 1, publicKey, 0, raw.Length - 5);

            _addressBase58 = addressBase58;
            ECPublicKey = new ECPublicKey(publicKey);
        }

        /// <summary>
        ///     Create a RadixAddress from a Elliptic Curve Public Key
        /// </summary>
        /// <param name="magic"></param>
        /// <param name="publicKey"></param>
        public RadixAddress(int magic, ECPublicKey publicKey)
        {
            if (publicKey == null)
                throw new ArgumentNullException(nameof(publicKey));

            if (publicKey.Length() != 33)
                throw new ArgumentException($"publickey must be 33 in lenghth but was : {publicKey.Length()}");

            byte[] addressBytes = new byte[1 + publicKey.Length() + 4];
            // Universe magic byte
            addressBytes[0] = (byte)(magic & 0xff);
            publicKey.CopyPublicKey(addressBytes, 1);

            // Checksum
            byte[] check = RadixHash.From(addressBytes, 0, publicKey.Length() + 1).ToByteArray();
            Array.Copy(check, 0, addressBytes, publicKey.Length() + 1, 4);

            //_addressBase58 = Base58.ToBase58(addressBytes);
            _addressBase58 = Base58Encoding.Encode(addressBytes);
            ECPublicKey = publicKey;
        }

        public override string ToString()
        {
            return _addressBase58;
        }        

        public override bool Equals(object obj)
        {
            if (!(obj is RadixAddress))
                return false;

            var adr = (RadixAddress)obj;

            return adr._addressBase58 == _addressBase58;
        }

        public override int GetHashCode()
        {
            return -1408821578 + EqualityComparer<string>.Default.GetHashCode(_addressBase58);
        }
    }
}