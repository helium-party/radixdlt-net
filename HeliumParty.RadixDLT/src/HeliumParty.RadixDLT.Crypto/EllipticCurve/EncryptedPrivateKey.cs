using HeliumParty.RadixDLT.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT.EllipticCurve
{
    public class EncryptedPrivateKey : IBase64Encoded
    {
        private readonly byte[] _encryptedKey;

        public EncryptedPrivateKey(byte[] encryptedKey)
        {
            _encryptedKey = encryptedKey;
        }

        public EncryptedPrivateKey(string base64Encryptedkey)
            : this (Convert.FromBase64String(base64Encryptedkey))
        {
        }

        public string Base64 => Convert.ToBase64String(_encryptedKey);

        public byte[] Base64Array => Arrays.CopyOfRange(_encryptedKey, 0, _encryptedKey.Length);
    }
}
