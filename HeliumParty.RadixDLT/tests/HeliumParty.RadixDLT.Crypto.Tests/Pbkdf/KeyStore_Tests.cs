using HeliumParty.RadixDLT.Crypto.Tests.Resources;
using HeliumParty.RadixDLT.EllipticCurve;
using HeliumParty.RadixDLT.EllipticCurve.Managers;
using HeliumParty.RadixDLT.Pbkdf;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace HeliumParty.RadixDLT.Crypto.Tests.Pbkdf
{    
    public class KeyStore_Tests
    {
        private const string VALID_PRIVKEY_INBASE64 = "lA8N6h4uUEbmf+Pp4DS41UPBJ8LIlUwBkfjKThw0fuI=";
        private const string VALID_PUBKEY_INBASE64 = "A3eCL5NJVVmJLXloK+zO9BFj36sHKGHHxG6Ytz5DX+qr";
        private const string KEYSTORE_PASSPHRASE = "test";

        private readonly KeyStore _store;
        private readonly IECKeyManager _keyMan;

        public KeyStore_Tests()
        {
            _store = JsonConvert.DeserializeObject<KeyStore>
                (ResourceParser.GetResource("keystoretest.json"));

            _keyMan = new ECKeyManager();
        }


        /// <summary>
        ///     We need to closely test this because any failure will cause a user to loose it's private key
        /// </summary>
        [Fact]
        public void KeyStore_Should_SerializePerfectly()
        {
            _store.ShouldNotBeNull();
            _store.Id.ShouldBe("51dac59689854c3c265696c5dd4cef57");

            _store.CryptoDetails.ShouldNotBeNull();
            _store.CryptoDetails.CipherParams.IV.ShouldBe("64455195780e952aba831f77394eceb7");
            _store.CryptoDetails.Cipher.ShouldBe("aes-256-ctr");
            _store.CryptoDetails.CipherText.ShouldBe("5016afca1361590792f548c6a6e99f8b85c4c3471aa1ae784f61010034873b3948aaebee56c5d45658e9bdebe5e635ea56891bad1802a7ae0c05b679d79f9e02");
            _store.CryptoDetails.Mac.ShouldBe("cbd2d1661d66836bd93c6f855ea019d409af880f1a2a8318edeb3f2c34ce802f");

            _store.CryptoDetails.Pbkdfparams.ShouldNotBeNull();
            _store.CryptoDetails.Pbkdfparams.Iterations.ShouldBe(100000);
            _store.CryptoDetails.Pbkdfparams.KeyLength.ShouldBe(32);
            _store.CryptoDetails.Pbkdfparams.Digest.ShouldBe("sha512");
            _store.CryptoDetails.Pbkdfparams.Salt.ShouldBe("7ea95d11dc015939e32876a7e764580d5f38bf867eaacd47cd206d10ae05f02b");



        }

        [Fact]
        public void KeyStore_Should_GenerateValidPrivKey()
        {
            //act
            var privKey = PrivateKeyEncrypter.Decrypt(KEYSTORE_PASSPHRASE, _store);

            // assert
            privKey.ShouldNotBeNull();
            privKey.Base64.ShouldBe(VALID_PRIVKEY_INBASE64);
        }
    }
}
