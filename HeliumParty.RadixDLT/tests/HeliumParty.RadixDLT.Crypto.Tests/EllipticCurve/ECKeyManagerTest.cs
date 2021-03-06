﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeliumParty.BaseTest;
using HeliumParty.RadixDLT.EllipticCurve;
using HeliumParty.RadixDLT.EllipticCurve.Managers;
using Shouldly;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace HeliumParty.RadixDLT.Crypto.Tests
{
    public class ECKeyManagerTest : HbIntegratedBaseTest
    {
        private readonly IECKeyManager _manager;

        public ECKeyManagerTest()
        {
            _manager = IocContainer.GetService<IECKeyManager>();
        }

        [Fact]
        public void Should_GenerateUnique_Keys()
        {
            var keyPairs = new List<ECKeyPair>();

            var i = 0;
            while (i++ < 100)
                keyPairs.Add(_manager.GetRandomKeyPair());

            foreach (var pair in keyPairs)
            {
                var a = keyPairs.Where(ec =>
                    ec.PrivateKey.Base64 == pair.PrivateKey.Base64 ||
                    ec.PublicKey.Base64 == pair.PublicKey.Base64
                ).ToList();

                //there should only be one match, aka himself
                a.Count.ShouldBe(1);
            }
        }


        [Fact]
        public void Should_CreateTheSame_KeyPair()
        {
            var pair = _manager.GetRandomKeyPair();
            var pair2 = _manager.GetKeyPair(pair.PrivateKey);

            pair2.PublicKey.Base64.ShouldBe(pair2.PublicKey.Base64);
            _manager.VerifyKeyPair(pair2).ShouldBe(true);
        }

        [Fact]
        public void Should_CreateAValidKeyPair()
        {
            //arrange
            var privkeyBase64 = "lA8N6h4uUEbmf+Pp4DS41UPBJ8LIlUwBkfjKThw0fuI=";
            var pubkeyBase64 = "A3eCL5NJVVmJLXloK+zO9BFj36sHKGHHxG6Ytz5DX+qr";            

            var privKey = new ECPrivateKey(Convert.FromBase64String(privkeyBase64));
            //var addressbase58 = "JH11YADXCXF84r5G6ZVt6QkrsXNBa4X62rpTyaRiVWGrqTZrcjS";

            //act
            var pair = _manager.GetKeyPair(privKey);

            //assert
            pair.PublicKey.Base64.ShouldBe(pubkeyBase64);
        }

        [Fact]
        public void Should_CreateValid_ECSignature()
        {
            var toSign = "Hello World!";
            var pair = _manager.GetRandomKeyPair();

            var signature = _manager.GetECSignature(pair.PrivateKey, Encoding.ASCII.GetBytes(toSign));
            _manager.VerifyECSignature(pair.PublicKey, signature, Encoding.ASCII.GetBytes(toSign)).ShouldBe(true);
            _manager.VerifyECSignature(pair.PublicKey, signature, Encoding.ASCII.GetBytes(toSign + " ")).ShouldBe(false);
        }

        [Fact]
        public void UnEncrypted_And_DeCrypted_ShouldBeTheSame()
        {
            //arrange
            var keypair = _manager.GetRandomKeyPair();
            var msg = "This is a message we want to encrypt";
            var toEncrypt = RadixConstants.StandardEncoding.GetBytes(msg);

            //act
            var encrypted = _manager.Encrypt(keypair.PublicKey, toEncrypt);
            var decrypted = _manager.Decrypt(keypair.PrivateKey, encrypted);

            //assert
            msg.ShouldBe(RadixConstants.StandardEncoding.GetString(decrypted));
            msg.ShouldNotBe(RadixConstants.StandardEncoding.GetString(encrypted));
        }
    }
}