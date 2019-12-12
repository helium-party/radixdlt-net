using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using HeliumParty.RadixDLT.Primitives;
using Org.BouncyCastle.Math;

namespace HeliumParty.RadixDLT.Hashing
{
    public class RadixHash
    {
        private readonly byte[] _hash;

        private RadixHash(byte[] hash)
        {
            _hash = hash ?? throw new ArgumentNullException(nameof(hash));
        }
        public static RadixHash From(byte[] raw)
        {
            using (SHA256 hash = SHA256.Create())
            {
                return new RadixHash(hash.ComputeHash(hash.ComputeHash(raw)));
            }
        }
        public static RadixHash From(byte[] raw, int offset, int length)
        {
            using (SHA256 hash = SHA256.Create())
            {
                return new RadixHash(hash.ComputeHash(hash.ComputeHash(raw, offset, length)));
            }
        }

        public void CopyTo(byte[] array, int offset)
        {
            CopyTo(array, offset, _hash.Length);
        }
        public void CopyTo(byte[] array, int offset, int length)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (array.Length - offset < _hash.Length)
                throw new ArgumentException($"Array must be bigger than offset {_hash.Length} but was {array.Length}");

            Array.Copy(_hash, 0, array, offset, length);
        }

        public static RadixHash Sha512Of(byte[] raw)
        {
            using (SHA512 hash = SHA512.Create())
            {
                return new RadixHash(hash.ComputeHash(hash.ComputeHash(raw)));
            }
        }

        public byte this[int index] => _hash[index];

        public override bool Equals(object o)
        {
            if (o == this)
                return true;

            if (!(o.GetType() == typeof(RadixHash)))
                return false;

            return _hash.SequenceEqual(((RadixHash)o)._hash);
        }

        public override int GetHashCode()
        {
            return new BigInteger(1, _hash).GetHashCode();
        }

        public override string ToString()
        {
            return Convert.ToBase64String(_hash);
        }

        public virtual string ToHexString() => Bytes.ToHexString(_hash);

        public virtual byte[] ToByteArray()
        {
            byte[] result = new byte[_hash.Length];
            _hash.CopyTo(result, 0);
            return result;
        }
    }
}