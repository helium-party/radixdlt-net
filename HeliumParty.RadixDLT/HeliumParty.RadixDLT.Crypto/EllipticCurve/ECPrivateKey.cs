using System;

namespace HeliumParty.RadixDLT.EllipticCurve
{
    public class ECPrivateKey : IBase64Encoded
    {
        private readonly byte[] _privateKey;

        public virtual string Base64 => Convert.ToBase64String(_privateKey);

        public virtual byte[] Base64Array
        {
            get
            {
                var arr = new byte[_privateKey.Length];
                _privateKey.CopyTo(arr, 0);
                return arr;
            }
        }

        public ECPrivateKey(byte[] privateKey)
        {
            _privateKey = new byte[privateKey.Length];
            Array.Copy(privateKey, _privateKey, privateKey.Length);
        }
    }
}