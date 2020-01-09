using System;
using System.Collections.Generic;
using System.Linq;
using HeliumParty.BaseTest;
using HeliumParty.RadixDLT.EllipticCurve;
using HeliumParty.RadixDLT.EllipticCurve.Managers;
using Shouldly;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace HeliumParty.RadixDLT.Crypto.Tests
{
    public class ECKeyManagerTest : HpIntegratedBaseTest
    {
        private readonly IECKeyManager _ecKeyManager;

        public ECKeyManagerTest()
        {
            _ecKeyManager = IocContainer.GetService<IECKeyManager>();
        }

        [Fact]
        public void Should_GenerateUnique_Keys()
        {
            //arrange
            var keyPairs = new List<ECKeyPair>();

            //act
            var i = 0;
            while (i++ < 100)
                keyPairs.Add(_ecKeyManager.GetRandomKeyPair());

            foreach (var pair in keyPairs)
            {
                var a = keyPairs.Where(ec =>
                    ec.PrivateKey.Base64 == pair.PrivateKey.Base64 ||
                    ec.PublicKey.Base64 == pair.PublicKey.Base64
                ).ToList();

                //assert
                //there should only be one match, aka himself
                a.Count.ShouldBe(1);
            }
        }

        [Fact]
        public void Should_CreateTheSame_KeyPair()
        {
            //arrange
            var pair = _ecKeyManager.GetRandomKeyPair();

            //act
            var pair2 = _ecKeyManager.GetKeyPair(pair.PrivateKey);

            //assert
            pair.PublicKey.Base64.ShouldBe(pair2.PublicKey.Base64);
            _ecKeyManager.VerifyKeyPair(pair).ShouldBe(true);
            _ecKeyManager.VerifyKeyPair(pair2).ShouldBe(true);
        }

        [Fact]
        public void Should_CreateAValidKeyPair()
        {
            //arrange
            var privkeyBase64 = "lA8N6h4uUEbmf+Pp4DS41UPBJ8LIlUwBkfjKThw0fuI=";
            var pubkeyBase64 = "A3eCL5NJVVmJLXloK+zO9BFj36sHKGHHxG6Ytz5DX+qr";            

            var privKey = new ECPrivateKey(Convert.FromBase64String(privkeyBase64));

            //act
            var pair = _ecKeyManager.GetKeyPair(privKey);

            //assert
            pair.PublicKey.Base64.ShouldBe(pubkeyBase64);
        }

        [Fact]
        public void Should_CreateValid_ECSignature()
        {
            //arrange
            var toSign = "Hello World!";
            var pair = _ecKeyManager.GetRandomKeyPair();

            //act
            var signature = _ecKeyManager.GetECSignature(pair.PrivateKey, RadixConstants.StandardEncoding.GetBytes(toSign));

            //assert
            _ecKeyManager.VerifyECSignature(pair.PublicKey, signature, RadixConstants.StandardEncoding.GetBytes(toSign)).ShouldBe(true);
            _ecKeyManager.VerifyECSignature(pair.PublicKey, signature, RadixConstants.StandardEncoding.GetBytes(toSign + " ")).ShouldBe(false);
        }

        [Fact]
        public void UnEncrypted_And_DeCrypted_ShouldBeTheSame()
        {
            //arrange
            var keypair = _ecKeyManager.GetRandomKeyPair();
            var msg = "This is a message we want to encrypt";
            var toEncrypt = RadixConstants.StandardEncoding.GetBytes(msg);

            //act
            var encrypted = _ecKeyManager.Encrypt(keypair.PublicKey, toEncrypt);
            var decrypted = _ecKeyManager.Decrypt(keypair.PrivateKey, encrypted);

            //assert
            msg.ShouldBe(RadixConstants.StandardEncoding.GetString(decrypted));
            msg.ShouldNotBe(RadixConstants.StandardEncoding.GetString(encrypted));
        }

        [Fact]
        public void Should_Throw_Exception_On_Invalid_Data()
        {
            //arrange
            var keypair = _ecKeyManager.GetRandomKeyPair();

            //assert
            Should.Throw<Exception>(() => _ecKeyManager.Decrypt(keypair.PrivateKey, new byte[] {0x00}));
        }
    }
}