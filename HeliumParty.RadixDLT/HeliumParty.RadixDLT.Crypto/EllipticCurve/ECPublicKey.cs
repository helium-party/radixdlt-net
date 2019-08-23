using System;

namespace HeliumParty.RadixDLT.EllipticCurve
{
    public class ECPublicKey : IBase64Encoded
    {
        private readonly byte[] _publicKey;

        public string Base64 => Convert.ToBase64String(_publicKey);

        public byte[] Base64Array
        {
            get
            {
                var arr = new byte[_publicKey.Length];
                _publicKey.CopyTo(arr, 0);
                return arr;
            }
        }

        public ECPublicKey(byte[] publicKey)
        {
            _publicKey = new byte[publicKey.Length];
            Array.Copy(publicKey, _publicKey, publicKey.Length);
        }

        public virtual int Length()
        {
            return _publicKey.Length;
        }

        public virtual void CopyPublicKey(byte[] dest, int destPos)
        {
            Array.Copy(_publicKey, 0, dest, destPos, _publicKey.Length);
        }

        public override string ToString() => Base64;
    }
}