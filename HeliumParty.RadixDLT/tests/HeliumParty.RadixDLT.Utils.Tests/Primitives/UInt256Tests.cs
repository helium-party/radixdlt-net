using HeliumParty.RadixDLT.Primitives;
using Shouldly;
using Xunit;

namespace HeliumParty.RadixDLT.Utils.Tests.Primitives
{
    public class UInt256Tests
    {
        [Fact]
        public void Should_Convert_IntToByte()
        {
            //arrange
            var bytes = RandomGenerator.GetRandomBytes(32);

            //act
            UInt256 bigint = bytes;
            byte[] bytesnew = bigint;

            //assert
            bytes.ShouldBe(bytesnew);
        }
    }
}
