using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HeliumParty.DependencyInjection;
using HeliumParty.RadixDLT.Exceptions;
using HeliumParty.RadixDLT.Hashing;
using HeliumParty.RadixDLT.Primitives;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;

namespace HeliumParty.RadixDLT.EllipticCurve.Managers
{
    public class ECKeyManager : IECKeyManager , ITransientDependency
    {
        private const string Curvealgo = "secp256k1";
        private const string Keypairalgo = "ECDSA";

        public virtual ECSignature GetECSignature(ECPrivateKey privateKey, byte[] data, bool beDeterministic = false, bool enforceLowS = true)
        {
            var curve = SecNamedCurves.GetByName(Curvealgo);
            var domain = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);


            IDsaKCalculator kCalculator;

            if (beDeterministic)
                kCalculator = new HMacDsaKCalculator(new Sha256Digest());
            else kCalculator = new RandomDsaKCalculator();

            ECDsaSigner signer = new ECDsaSigner(kCalculator);
            signer.Init(true, new ECPrivateKeyParameters(new BigInteger(1, privateKey.Base64Array), domain));

            BigInteger[] components = signer.GenerateSignature(data);

            BigInteger r = components[0];
            BigInteger s = components[1];

            BigInteger curveOrder = domain.N;
            BigInteger halvCurveOrder = curveOrder.ShiftRight(1);

            bool sIsLow = s.CompareTo(halvCurveOrder) <= 0;

            if (enforceLowS && !sIsLow)
            {
                s = curveOrder.Subtract(s);
            }

            return new ECSignature(r, s);
        }

        public virtual ECKeyPair GetKeyPair(ECPrivateKey privatekey, bool compressed = true)
        {
            var curve = ECNamedCurveTable.GetByName(Curvealgo);
            var domainParams = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H, curve.GetSeed());

            BigInteger d = new BigInteger(1, privatekey.Base64Array);
            ECPoint q = domainParams.G.Multiply(d);

            var publicParams = new ECPublicKeyParameters(q, domainParams);
            var pubkey = new ECPublicKey(publicParams.Q.GetEncoded(compressed));

            return new ECKeyPair(privatekey, pubkey);
        }

        public virtual ECKeyPair GetRandomKeyPair(bool compressed = true)
        {
            var curve = ECNamedCurveTable.GetByName(Curvealgo);
            var domainParams = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H, curve.GetSeed());

            var secureRandom = new SecureRandom();
            var keyParams = new ECKeyGenerationParameters(domainParams, secureRandom);

            var generator = new ECKeyPairGenerator(Keypairalgo);
            generator.Init(keyParams);
            var keyPair = generator.GenerateKeyPair();


            var privateKey = (keyPair.Private as ECPrivateKeyParameters).D.ToByteArray();
            //var privateKey = (keyPair.Private as ECPrivateKeyParameters).D.ToByteArrayUnsigned();
            var publicKey = (keyPair.Public as ECPublicKeyParameters).Q.GetEncoded(compressed);

            return new ECKeyPair(privateKey, publicKey);
        }

        public virtual bool VerifyECSignature(ECPublicKey publicKey, ECSignature signature, byte[] data)
        {
            var curve = ECNamedCurveTable.GetByName(Curvealgo);
            var domainParams = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H, curve.GetSeed());

            ECDsaSigner verifier = new ECDsaSigner();
            verifier.Init(false, new ECPublicKeyParameters(domainParams.Curve.DecodePoint(publicKey.Base64Array), domainParams));

            return verifier.VerifySignature(data, signature.RInt, signature.SInt);
        }

        public virtual bool VerifyKeyPair(ECKeyPair keyPair)
        {
            var pair = GetKeyPair(keyPair.PrivateKey);

            return pair.PublicKey.Base64Array.SequenceEqual(keyPair.PublicKey.Base64Array);
        }

        public virtual byte[] Encrypt(ECPublicKey publicKey, byte[] data)
        {
            Random rand = new SecureRandom();

            byte[] iv = new byte[16];
            // 2. Generate 16 random bytes using a secure random number generator. Call them IV
            rand.NextBytes(iv);

            var randomKeyPair = GetRandomKeyPair();

            //  Do an EC point multiply with publicKey and random keypair. This gives you a point M.
            var m = GetECPoint(publicKey).Multiply(new BigInteger(1, randomKeyPair.PrivateKey.Base64Array)).Normalize();

            //  Use the X component of point M and calculate the SHA512 hash H.
            byte[] h = RadixHash.Sha512Of(m.AffineXCoord.GetEncoded()).ToByteArray();

            //  The first 32 bytes of H are called key_e and the last 32 bytes are called key_m.
            byte[] keyE = Arrays.CopyOfRange(h, 0, 32);
            byte[] keyM = Arrays.CopyOfRange(h, 32, 64);
            byte[] encrypted = Crypt(true, data, iv, keyE);

            //  Calculate a 32 byte MAC with HMACSHA256, using key_m as salt and
            //  IV + ephemeral.pub + cipher text as data. Call the output MAC.
            byte[] mac = CalculateMAC(keyM, iv, randomKeyPair.PublicKey, encrypted);

            //  Write out the encryption result IV +ephemeral.pub + encrypted + MAC
            using (var memstr = new MemoryStream())
            {
                memstr.Write(iv, 0, iv.Length);
                memstr.WriteByte((byte)randomKeyPair.PublicKey.Length());
                memstr.Write(randomKeyPair.PublicKey.Base64Array, 0, randomKeyPair.PublicKey.Length());
                memstr.Write(BitConverter.GetBytes(encrypted.Length), 0, 4);
                memstr.Write(encrypted, 0, encrypted.Length);
                memstr.Write(mac, 0, mac.Length);

                return memstr.ToArray();
            }
        }

        protected virtual byte[] Crypt(bool encrypt, byte[] data, byte[] iv, byte[] keyE)
        {
            //  Pad the input text to a multiple of 16 bytes, in accordance to PKCS7.
            BufferedBlockCipher cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new AesEngine()), new Pkcs7Padding());

            ICipherParameters parms = new ParametersWithIV(new KeyParameter(keyE), iv);

            //  Encrypt the data with AES - 256 - CBC, using IV as initialization vector,
            //  key_e as encryption key and the padded input text as payload. Call the output cipher text.
            cipher.Init(encrypt, parms);

            byte[] buffer = new byte[cipher.GetOutputSize(data.Length)];

            int length = cipher.ProcessBytes(data, 0, data.Length, buffer, 0);
            length += cipher.DoFinal(buffer, length);

            byte[] encrypted;
            if (length < buffer.Length)
                encrypted = Arrays.CopyOfRange(buffer, 0, length);
            else encrypted = buffer;
            return encrypted;
        }

        public virtual byte[] Decrypt(ECPrivateKey privateKey, byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                //  read the IV
                byte[] iv = new byte[16];
                stream.Read(iv, 0, iv.Length);

                //  read the publickey
                var pubkeysize = stream.ReadByte();
                byte[] pubkeyraw = new byte[pubkeysize];
                stream.Read(pubkeyraw, 0, pubkeyraw.Length);
                var pubkey = new ECPublicKey(pubkeyraw);

                //  Do an EC point multiply with this.getPrivateKey() and ephemeral public key. This gives you a point M.
                var m = GetECPoint(pubkey).Multiply(new BigInteger(1, privateKey.Base64Array)).Normalize();

                //  Use the X component of point M and calculate the SHA512 hash H.
                byte[] h = RadixHash.Sha512Of(m.XCoord.GetEncoded()).ToByteArray();

                //  The first 32 bytes of H are called key_e and the last 32 bytes are called key_m.
                byte[] keyE = Arrays.CopyOfRange(h, 0, 32);
                byte[] keyM = Arrays.CopyOfRange(h, 32, 64);

                //  Read encrypted data
                var size = new byte[4];
                stream.Read(size, 0, size.Length);
                byte[] encrypted = new byte[BitConverter.ToInt32(size, 0)];
                stream.Read(encrypted, 0, encrypted.Length);

                //  Read MAC
                byte[] mac = new byte[32];
                stream.Read(mac, 0, mac.Length);

                //  Compare MAC with MAC'. If not equal, decryption will fail.
                byte[] pkMac = CalculateMAC(keyM, iv, pubkey, encrypted);
                if (!pkMac.SequenceEqual(mac))
                    throw new MacMismatchException(mac,pkMac);

                //  Decrypt the cipher text with AES-256-CBC, using IV as initialization vector, key_e as decryption key,
                //  and the cipher text as payload. The output is the padded input text.
                return Crypt(false, encrypted, iv, keyE);
            }
        }

        public virtual byte[] CalculateMAC(byte[] salt, byte[] iv, ECPublicKey publicKey, byte[] encrypted)
        {
            var hmac = new HMac(new Sha256Digest());
            hmac.Init(new KeyParameter(salt));
            byte[] result = new byte[hmac.GetMacSize()];

            using (var stream = new MemoryStream())
            {
                stream.Write(iv, 0, iv.Length);
                stream.Write(publicKey.Base64Array, 0, publicKey.Length());
                stream.Write(encrypted, 0, encrypted.Length);

                var msg = stream.ToArray();

                hmac.BlockUpdate(msg, 0, msg.Length);
            }

            hmac.DoFinal(result, 0);

            return result;
        }

        public virtual ECPoint GetECPoint(ECPublicKey publicKey)
        {
            var pubKey = publicKey.Base64Array;
            int domainSize = pubKey[0] == 4 ? ((pubKey.Length / 2) - 1) * 8 : (pubKey.Length - 1) * 8;

            var curve = ECNamedCurveTable.GetByName(Curvealgo);
            var domainParams = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H, curve.GetSeed());

            return domainParams.Curve.DecodePoint(pubKey);
        }
        
        public virtual ECPrivateKey DecryptSharedKey(ECPrivateKey privateKey, EncryptedPrivateKey sharedKey)
        {
            if (privateKey == null)
                throw new ArgumentException($"{nameof(privateKey)} cannot be null");

            byte[] sharedprivateKey = Decrypt(privateKey,sharedKey.Base64Array);

            return new ECPrivateKey(sharedprivateKey);
        }

        public virtual byte[] DecryptWithSharedKey(ECPrivateKey privateKey , EncryptedPrivateKey sharedKey, byte[] data)
        {
            return Decrypt(DecryptSharedKey(privateKey, sharedKey), data);
        }

        public virtual EncryptedPrivateKey CreateSharedKey(ECPublicKey foreignPubKey, ECPrivateKey keyToShare)
        {
            return new EncryptedPrivateKey(Encrypt(foreignPubKey, keyToShare.Base64Array),foreignPubKey);
        }
    }
}