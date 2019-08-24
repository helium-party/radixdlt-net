using System;

namespace HeliumParty.RadixDLT.Primitives
{
    /// <summary>
    /// Utilities for manipulating primitive <see cref="int"/> values.
    /// </summary>
    public static class Ints
    {
        /// <summary>
        /// Writes the byte value of an int to a byte array with 0 offset, big-endian
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>The byte array with the value</returns>
        public static byte[] ToByteArray(int value) => CopyTo(value, new byte[sizeof(int)], 0);

        /// <summary>
        /// Writes the byte value of an int to a byte array with an offset, big-endian
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <param name="offset">The offset at which to write the value</param>
        /// <returns>The byte array with the value</returns>
        public static byte[] CopyTo(int value, byte[] bytes, int offset)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            for (int i = offset + sizeof(int) - 1; i >= offset; i--)
            {
                bytes[i] = (byte)(value & 0xFF);
                value >>= 8;
            }
            return bytes;
        }

        /// <summary>
        /// Decode an integer from an byte array with an offset
        /// </summary>
        /// <param name="bytes">The byte array to decode to an integer</param>
        /// <param name="offset">The offset within the array to start decoding</param>
        /// <returns>The decoded integer value</returns>
        public static int FromByteArray(byte[] bytes, int offset = 0)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            var value = 0;
            for (int i = 0; i < sizeof(int); i++)
            {
                value <<= 8;
                value |= bytes[offset + i] & 0xFF;
            }

            return value;
        }

        /// <summary>
        /// Assemble an int from four bytes, big-endian
        /// </summary>
        /// <param name="b0">Most significant byte</param>
        /// <param name="b1">Next most significant byte</param>
        /// <param name="b2">Next least significant byte</param>
        /// <param name="b3">Least significant byte</param>
        /// <returns>The int value</returns>
        public static int FromBytes(byte b0, byte b1, byte b2, byte b3) => b0 << 24 | (b1 & 0xFF) << 16 | (b2 & 0xFF) << 8 | (b3 & 0xFF);
    }
}