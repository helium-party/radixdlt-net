using HeliumParty.RadixDLT.Crypto.Tests.Resources;
using HeliumParty.RadixDLT.EllipticCurve;
using HeliumParty.RadixDLT.EllipticCurve.Managers;
using HeliumParty.RadixDLT.Encryption;

using Newtonsoft.Json;
using Shouldly;
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
            _store.Id.ShouldBe("37f64b70460674c17cb5a7f1745c2b04");

            _store.CryptoDetails.ShouldNotBeNull();
            _store.CryptoDetails.CipherParams.IV.ShouldBe("98190c80b8c4c592ff5a5ed62e4b0975");
            _store.CryptoDetails.Cipher.ShouldBe("aes-256-ctr");
            _store.CryptoDetails.CipherText.ShouldBe("b0772ad140ab17eb199f7d7f35b2417d7eb1c1b788b0f77bb03ee1dec37f527897ad6a182f9e46e7a7383024c663d62dfb3e529e9f94982e6ec512ad0621acbf");
            _store.CryptoDetails.Mac.ShouldBe("3dfeb66ed3bda00f1bcc3ba6c36e9cb9cc19d4a3552c8c2f8675853a92d15d75");

            _store.CryptoDetails.Pbkdfparams.ShouldNotBeNull();
            _store.CryptoDetails.Pbkdfparams.Iterations.ShouldBe(100000);
            _store.CryptoDetails.Pbkdfparams.KeyLength.ShouldBe(32);
            _store.CryptoDetails.Pbkdfparams.Digest.ShouldBe("sha512");
            _store.CryptoDetails.Pbkdfparams.Salt.ShouldBe("8484539e8aac5506030de872a437faf15377e36b031130cd2751d5adb996bbbf");



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

        [Fact]
        public void Should_Generate_Decryptable_KeyStore()
        {
            //arrange 
            var privKey = new ECPrivateKey(RadixConstants.StandardEncoding.GetBytes(VALID_PRIVKEY_INBASE64));

            //act
            var store = PrivateKeyEncrypter.Encrypt(KEYSTORE_PASSPHRASE, privKey);
            var decryptedKey = PrivateKeyEncrypter.Decrypt(KEYSTORE_PASSPHRASE, store);

            //assert
            decryptedKey.ShouldNotBeNull();
            decryptedKey.Base64.ShouldBe(privKey.Base64);
        }
    }
}
