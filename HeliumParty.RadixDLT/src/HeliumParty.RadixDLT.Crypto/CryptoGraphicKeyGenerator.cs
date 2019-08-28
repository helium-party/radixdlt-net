using HeliumParty.RadixDLT.Primitives;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace HeliumParty.RadixDLT
{
    public static class CryptoGraphicKeyGenerator
    {
        public static byte[] Generate(string password)
        {
            byte[] salt;
            byte[] key;
            using (var derivedBytes = new Rfc2898DeriveBytes(password, 16, iterations: 50000))
            {
                salt = derivedBytes.Salt;
                key = derivedBytes.GetBytes(16);               
            }

            return Arrays.ConcatArrays(key, salt);
        }
    }
}
