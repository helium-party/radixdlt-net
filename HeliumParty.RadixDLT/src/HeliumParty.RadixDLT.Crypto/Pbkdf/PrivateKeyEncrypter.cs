using HeliumParty.RadixDLT.EllipticCurve;
using HeliumParty.RadixDLT.Exceptions;
using HeliumParty.RadixDLT.Hashing;
using HeliumParty.RadixDLT.Primitives;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeliumParty.RadixDLT.Pbkdf
{
    public static class PrivateKeyEncrypter
    {
        public static ECPrivateKey Decrypt(string password, KeyStore keystore)
        {
            byte[] salt = RadixConstants.StandardEncoding.GetBytes(keystore.CryptoDetails.Pbkdfparams.Salt);
            byte[] iv = Bytes.FromHexString(keystore.CryptoDetails.CipherParams.IV);
            byte[] mac = Bytes.FromHexString(keystore.CryptoDetails.Mac);
            byte[] ciphertext = Bytes.FromHexString(keystore.CryptoDetails.CipherText);
            var iterations = keystore.CryptoDetails.Pbkdfparams.Iterations;
            var keylength = keystore.CryptoDetails.Pbkdfparams.KeyLength;

            //get a derived key by salt
            var secrectkey = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA512, iterations, keylength);

            // init cipher
            IBufferedCipher cipher = CipherUtilities.GetCipher("AES/CTR/NoPadding");
            cipher.Init(false, new ParametersWithIV(ParameterUtilities.CreateKeyParameter("AES", secrectkey ), iv));

            //calculate mac and compare
            byte[] computedMac = CalculateMac(secrectkey, ciphertext);
            if (!computedMac.SequenceEqual(mac))
                throw new MacMismatchException(mac, computedMac);

            var decryptedText = RadixConstants.StandardEncoding.GetString(cipher.DoFinal(ciphertext));
            
            return new ECPrivateKey(Bytes.FromHexString(decryptedText));
        }

        private static byte[] CalculateMac(byte[] derivedKey, byte[] ciphertext)
        {
            byte[] result = Arrays.ConcatArrays(derivedKey, ciphertext);//new byte[derivedKey.Length + ciphertext.Length];

            return RadixHash.From(result).ToByteArray();
        }

        public static KeyStore Encrypt(string password, ECPrivateKey privatekey)
        {
            throw new NotImplementedException();
        }
    }
}
