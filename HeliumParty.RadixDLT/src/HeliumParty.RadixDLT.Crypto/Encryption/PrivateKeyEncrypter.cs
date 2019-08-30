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

namespace HeliumParty.RadixDLT.Encryption
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

        public static KeyStore Encrypt(string password, ECPrivateKey privatekey, int keyLength = 32, int iterations = 10000)
        {
            //generate salt
            var salt = RadixConstants.StandardEncoding.GetBytes(Bytes.ToHexString(RandomGenerator.GetRandomBytes(32)));


            var derivedKey = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA512, iterations, keyLength);
            

            // init cipher
            IBufferedCipher cipher = CipherUtilities.GetCipher("AES/CTR/NoPadding");

            Random rand = new SecureRandom();
            var iv = new byte[16];
            // 2. Generate 16 random bytes using a secure random number generator. Call them IV
            rand.NextBytes(iv);

            cipher.Init(true, new ParametersWithIV(ParameterUtilities.CreateKeyParameter("AES", derivedKey), iv));
            

            var privHexKey = Bytes.ToHexString(privatekey.Base64Array);
            var encryptedPrivKey = cipher.DoFinal(RadixConstants.StandardEncoding.GetBytes(privHexKey));
            var encryptedPrivKeyHex = Bytes.ToHexString(encryptedPrivKey);

            var mac = Bytes.ToHexString(CalculateMac(derivedKey, Bytes.FromHexString(encryptedPrivKeyHex)));

            return new KeyStore()
            {
                Id = Bytes.ToHexString(RandomGenerator.GetRandomBytes(16)),
                CryptoDetails = new CryptoDetails()
                {
                    Cipher = "aes-256-ctr",
                    CipherText = encryptedPrivKeyHex,
                    Mac = mac,
                    CipherParams = new CipherParams()
                    {
                        IV = Bytes.ToHexString(iv)
                    },
                    Pbkdfparams = new Pbkdfparams()
                    {
                        Iterations = iterations,
                        KeyLength = keyLength,
                        Digest = "sha512",
                        Salt = RadixConstants.StandardEncoding.GetString(salt)//Bytes.ToHexString(salt)
                    }
                }
            };
        }
        
    }
}
