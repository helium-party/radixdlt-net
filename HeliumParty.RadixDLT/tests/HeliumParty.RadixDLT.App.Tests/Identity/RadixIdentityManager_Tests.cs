using HeliumParty.RadixDLT.Identity;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace HeliumParty.RadixDLT.App.Tests.Identity
{
    public class RadixIdentityManager_Tests
    {
        private const string PRIVATEKEY_INBASE64 = "lA8N6h4uUEbmf+Pp4DS41UPBJ8LIlUwBkfjKThw0fuI=";
        private const string PUBLICKEY_INBASE64 = "A3eCL5NJVVmJLXloK+zO9BFj36sHKGHHxG6Ytz5DX+qr";
        private readonly IRadixIdentityManager _IdentityManager;

        public RadixIdentityManager_Tests()
        {
            _IdentityManager = new RadixIdentityManager();
        }

        [Fact]
        public void Should_Create_RadixIdentity_FromPrivateKeyBase64()
        {
            //arrange & act
            var identity = _IdentityManager.FromPrivateKeyBase64(PRIVATEKEY_INBASE64);

            //assert
            identity.PublicKey.ShouldNotBeNull();
            identity.PublicKey.Base64.ShouldBe(PUBLICKEY_INBASE64);
        }
    }
}
