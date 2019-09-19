using HeliumParty.RadixDLT.Primitives;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace HeliumParty.RadixDLT.Utils.Tests.Primitives
{
    public class UInt256_Tests
    {
        [Fact]
        public void IntToByteConversionTest()
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
