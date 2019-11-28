using HeliumParty.RadixDLT.Primitives;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Xunit;

namespace HeliumParty.RadixDLT.Utils.Tests.Primitives
{
    public class BigDecimal_Tests
    {
        [Fact]
        public void Implicit_And_Explicit_Operator_Test()
        {
            //act
            var bd = (BigDecimal)20.5;            
            var floored = bd.Floor();


            //assert
            bd.ShouldBe(20.5);
            floored.ShouldBe(20d);
        }

        [Fact]
        public void Convert_BigDecimal_To_UInt256_Test()
        {
            //arrange
            double basedbl = 33;
            var bd = (BigDecimal)basedbl;
            // we use byte conversion here to make sure we are testing 2 different ways
            UInt256 uitocompare = ((BigInteger)33).ToByteArray();

            //act
            var ui256 = (UInt256)bd;

            //assert
            ui256.ShouldBe(uitocompare);
        }
    }
}
