using HeliumParty.RadixDLT.Primitives;
using Shouldly;
using Xunit;

namespace HeliumParty.RadixDLT.Utils.Tests.Primitives
{
    public class Int128Test
    {
        [Fact]
        public void ShouldCompare()
        {
            //arrange
            var ilong1 = new Int128((long)3000);
            var ilong2 = new Int128((long)6000);
            var ilong3 = new Int128((long)-3000);
            var ilong4 = new Int128((long)-6000);
            var ilong5 = new Int128((ulong)3000);
            var ilong6 = new Int128((ulong)6000);

            var idec1 = new Int128(5000m);
            var idec2 = new Int128(1100000m);
            var idec3 = new Int128(-5000m);
            var idec4 = new Int128(-1100000m);

            var iflo1 = new Int128(555.55d);
            var iflo2 = new Int128(222.22d);
            var iflo3 = new Int128(-555.55d);
            var iflo4 = new Int128(-222.22d);

            //assert
            ilong1.ShouldBe(ilong2 / (long)2);
            ilong1.ShouldBe(ilong2 / (ulong)2);
            ilong1.ShouldBe(ilong2 / 2);
            ilong2.ShouldBe(ilong1 * 2);
            ilong2.ShouldBe(ilong1 * (long)2);
            ilong2.ShouldBe(ilong1 * (ulong)2);

            ilong3.ShouldBe(ilong4 / 2);
            ilong3.ShouldBe(ilong4 / (long)2);
            ilong3.ShouldBe(ilong4 / (ulong)2);
            ilong4.ShouldBe(ilong3 * (ulong)2);
            ilong4.ShouldBe(ilong3 * (long)2);
        }
    }
}