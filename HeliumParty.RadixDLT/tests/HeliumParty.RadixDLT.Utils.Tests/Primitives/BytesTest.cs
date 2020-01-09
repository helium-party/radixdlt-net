using System;
using System.Text;
using HeliumParty.RadixDLT.Primitives;
using Shouldly;
using Xunit;

namespace HeliumParty.RadixDLT.Utils.Tests.Primitives
{
    public class BytesTest
    {
        [Fact]
        public void Arrays_Should_Be_Equal()
        {
            //arrange
            byte[] array1 = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            byte[] array2 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            //assert
            Bytes.ArrayEquals(array1, 1, 5, array2, 0, 5).ShouldBeTrue();
            Bytes.ArrayEquals(array1, 1, 5, array2, 0, 4).ShouldBeFalse();
            Bytes.ArrayEquals(array1, 0, 5, array2, 0, 5).ShouldBeFalse();
        }

        [Fact]
        public void HashCode_Should_Be_Equal()
        {
            //arrange
            byte[] array1 = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            byte[] array2 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            //assert
            for (int i = 0; i < 10; i++)
                Bytes.HashCode(array1, 1, i).ShouldBe(Bytes.HashCode(array2, 0, i));
        }

        [Fact]
        public void Should_Convert_To_HexStringByte_Correctly()
        {
            //assert
            for (int i = 0; i < 0x100; i++)
            {
                var x = i.ToString("x2");
                var y = Bytes.ToHexString((byte)i);
                x.ShouldBe(y);
            }
        }

        [Fact]
        public void Should_Convert_To_HexStringByteArray_Correctly()
        {
            //arrange
            var bytes = new byte[256];

            //act
            var sb = new StringBuilder();
            for (int i = 0; i < 256; i++)
            {
                bytes[i] = (byte)i;
                sb.Append(i.ToString("x2"));
            }
            var x = sb.ToString();

            //assert
            x.ShouldBe(Bytes.ToHexString(bytes));
        }

        [Fact]
        public void Should_Convert_To_HexStringPartialByteArray_Correctly()
        {
            //assert
            var bytes = new byte[256];

            //act
            var sb = new StringBuilder();
            for (int i = 0; i < 256; i++)
            {
                bytes[i] = (byte)i;
                sb.Append(i.ToString("x2"));
            }
            var x = sb.ToString();

            //assert
            for (int i = 0; i < 256; i++)
                x.Substring(0, i * 2).ShouldBe(Bytes.ToHexString(bytes, 0, i));

            for (int i = 0; i < 200; i++)
                x.Substring(i * 2, 20).ShouldBe(Bytes.ToHexString(bytes, i, 10));
        }

        [Fact]
        public void Should_Convert_From_HexString()
        {
            //arrange
            byte[] expected1 = { 0xAA };

            //assert
            expected1.ShouldBe(Bytes.FromHexString("AA"));
            expected1.ShouldBe(Bytes.FromHexString("aa"));
            expected1.ShouldBe(Bytes.FromHexString("aA"));

            //arrange
            byte[] expected2 = { 0xAB, 0xCD };

            //assert
            expected2.ShouldBe(Bytes.FromHexString("ABCD"));
            expected2.ShouldBe(Bytes.FromHexString("abcd"));

            //arrange
            byte[] expected3 = { 0x0A, 0xBC, 0xDE };

            //assert
            expected3.ShouldBe(Bytes.FromHexString("ABCDE"));
            expected3.ShouldBe(Bytes.FromHexString("abcde"));

            //arrange
            byte[] expected4 = { 0xAB, 0xCD, 0xEF };

            //assert
            expected4.ShouldBe(Bytes.FromHexString("ABCDEF"));
            expected4.ShouldBe(Bytes.FromHexString("abcdef"));

            //arrange
            byte[] expected5 = { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF };

            //assert
            expected5.ShouldBe(Bytes.FromHexString("0123456789ABCDEF"));
            expected5.ShouldBe(Bytes.FromHexString("0123456789abcdef"));

            //assert
            Should.Throw<ArgumentException>(() => Bytes.FromHexString("!"));
            Should.Throw<ArgumentException>(() => Bytes.FromHexString(":"));
            Should.Throw<ArgumentException>(() => Bytes.FromHexString("["));
            Should.Throw<ArgumentException>(() => Bytes.FromHexString("~"));
        }

        [Fact]
        public void Should_TrimLeadingZeros()
        {
            //assert
            Bytes.TrimLeadingZeros(null).ShouldBe(null);

            var emptyBytes = new byte[0];
            emptyBytes.ShouldBe(Bytes.TrimLeadingZeros(emptyBytes));

            for (int i = 0; i < 255; i++)
            {
                var oneByte = new byte[1];
                oneByte[0] = (byte)i;
                oneByte.ShouldBe(Bytes.TrimLeadingZeros(oneByte));
            }

            for (int i = 0; i < 255; i++)
            {
                var oneByte = new byte[1];
                var twoBytes = new byte[2];
                oneByte[0] = (byte)i;
                twoBytes[1] = (byte)i;
                oneByte.ShouldBe(Bytes.TrimLeadingZeros(twoBytes));
            }

            byte[] noLeadingZeros = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            noLeadingZeros.ShouldBe(Bytes.TrimLeadingZeros(noLeadingZeros));

            var singleZero = new byte[1];
            var severalZeros = new byte[10];
            singleZero.ShouldBe(Bytes.TrimLeadingZeros(severalZeros));

            byte[] zeroRemoved = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            byte[] withZero = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            zeroRemoved.ShouldBe(Bytes.TrimLeadingZeros(withZero));
        }
    }
}