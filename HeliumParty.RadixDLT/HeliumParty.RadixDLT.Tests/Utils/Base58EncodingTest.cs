using Shouldly;
using Xunit;

namespace HeliumParty.RadixDLT.Tests.Utils
{
    public class Base58EncodingTest
    {
        private const string ValidBase58 = "JF42V22No24ekweEbLXa872yWydh2r2yM89hyq2pxjCmcQTwUPo";

        [Fact]
        public void Base58FromToTest()
        {
            var decoded = Base58Encoding.Decode(ValidBase58);
            var encoded = Base58Encoding.Encode(decoded);

            encoded.ShouldBe(ValidBase58);
        }
    }
}