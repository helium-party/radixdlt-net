using System;

namespace HeliumParty.RadixDLT.Primitives
{
    /// <summary>
    /// Utilities for manipulating primitive <see cref="long"/> values.
    /// </summary>
    public class Longs
    {
        /// <summary>
        /// Writes the byte value of an long to a byte array with 0 offset, big-endian
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>The byte array with the value</returns>
        public static byte[] ToByteArray(long value) => CopyTo(value, new byte[sizeof(long)], 0);

        /// <summary>
        /// Writes the byte value of an long to a byte array with an offset, big-endian
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <param name="bytes">The byte array to write to</param>
        /// <param name="offset">The offset at which to write the value</param>
        /// <returns>The byte array with the value</returns>
        public static byte[] CopyTo(long value, byte[] bytes, int offset)
        {
            for (int i = offset + sizeof(long) - 1; i >= offset; i--)
            {
                bytes[i] = (byte)(value & 0xFFL);
                value >>= 8;
            }
            return bytes;
        }

        /// <summary>
        /// Decode a long from an byte array with an offset
        /// </summary>
        /// <param name="bytes">The byte array to decode to a long</param>
        /// <param name="offset">The offset within the array to start decoding</param>
        /// <returns>The decoded long value</returns>
        public static long FromByteArray(byte[] bytes, int offset = 0)
        {
            //TODO why exception and not return 0L ?
            if (bytes == null)
                throw new ArgumentNullException("bytes is null for 'int' conversion");

            var value = 0L;
            for (int i = 0; i < sizeof(long); i++)
            {
                value <<= 8;
                value |= bytes[offset + i] & 0xFFL;
            }

            return value;
        }

        /// <summary>
        /// Assemble a long from four bytes, big-endian
        /// </summary>
        /// <param name="b0">Most significant byte</param>
        /// <param name="b1">Next most significant byte</param>
        /// <param name="b6">Next least significant byte</param>
        /// <param name="b7">Least significant byte</param>
        /// <returns>The long value</returns>
        public static long FromBytes(byte b0, byte b1, byte b2, byte b3, byte b4, byte b5, byte b6, byte b7) =>
            (b0 & 0xFFL) << 56 | (b1 & 0xFFL) << 48 | (b2 & 0xFFL) << 40 | (b3 & 0xFFL) << 32
            | (b4 & 0xFFL) << 24 | (b5 & 0xFFL) << 16 | (b6 & 0xFFL) << 8 | (b7 & 0xFFL);

        /// <summary>
        /// Compares two unsigned long values
        /// </summary>
        /// <param name="x">The first value to compare</param>
        /// <param name="y">The second value to compare</param>
        /// <returns>0 if x==y, 1 if x > y and -1 if x < y</returns>
        public static int CompareUnsigned(long x, long y)
        {
            var l = x + long.MinValue;
            return l.CompareTo(y + long.MinValue);
        }

        /// <summary>
        /// Shifts the value the specified amount to the right (basicly sets the highest bit to 0 instead of 1 when shifting negative values)
        /// </summary>
        /// <param name="value">The value to shift</param>
        /// <param name="shiftCount">The amount of bits to shift</param>
        /// <returns>The shifted value</returns>
        public static long ShiftUnsignedRight(long value, int shiftCount)
        {
            // TODO: Recheck if comment is ok...?
            // Other than Java, C# doesn't support the unsigned shifting character '>>>'.
            // However, we can just cast the signed value to an unsigned, shift the value and then
            // recast it to the signed value.
            // Note: Bits don't change when casting from unsigned to signed and vise-versa, therefore this is possible.

            // Difference to normal '>>' operator: 
            // When shifting positive values, there isn't any difference
            // When shifting negative values, the usual '>>' operator would set the highest bit to 1, to not lose the negative sign
            //    --> The unsigned one sets the highest bit to 0 instead
            return (long)((ulong)value >> shiftCount);
        }
    }
}