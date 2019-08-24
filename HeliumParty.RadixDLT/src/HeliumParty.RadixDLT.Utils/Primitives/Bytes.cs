using System;
using System.Text;

namespace HeliumParty.RadixDLT.Primitives
{
    /// <summary>
    /// Utilities for manipulating primitive byte arrays.
    /// </summary>
    public static class Bytes
    {
        private static readonly char[] HexChars = "0123456789abcdef".ToCharArray();

        /// <summary>
        /// Compare two byte array segments for equality.
        /// </summary>
        /// <param name="a1">The first array to compare</param>
        /// <param name="offset1">The offset within a1 to begin the comparison</param>
        /// <param name="length1">The quantity of elements in a1 to compare</param>
        /// <param name="a2">The second array to compare</param>
        /// <param name="offset2">The offset within a2 to begin the comparison</param>
        /// <param name="length2">The quantity of elements in a2 to compare</param>
        public static bool ArrayEquals(byte[] a1, int offset1, int length1, byte[] a2, int offset2, int length2)
        {
            if (length1 != length2)
                return false;

            for (int i = 0; i < length1; i++)
            {
                if (a1[offset1 + i] != a2[offset2 + i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Calculates the hash code of a byte array segment.
        /// </summary>
        /// <param name="a">The array for which to calculate the hash code.</param>
        /// <param name="offset">The offset within the array to start the calculation.</param>
        /// <param name="length">The number of bytes for which to calculate the hash code.</param>
        /// <returns>The hash code.</returns>
        public static int HashCode(byte[] a, int offset, int length)
        {
            var i = length;
            var hc = i + 1;
            while (--i >= 0)
            {
                hc *= 257;
                hc ^= a[offset + i];
            }

            return hc;
        }

        /// <summary>
        /// Convert a byte array to a string using the RadixConstants.StandardEncoding
        /// </summary>
        /// <param name="bytes">The bytes to convert</param>
        /// <returns>The string</returns>
        public static string ToString(byte[] bytes) => RadixConstants.StandardEncoding.GetString(bytes);

        private static char ToHexChar(int value) => HexChars[value & 0xF];

        /// <summary>
        /// Convert a byte into a two-digit hex string.
        /// </summary>
        /// <param name="b">The byte to convert</param>
        /// <returns>The converted string</returns>
        public static string ToHexString(byte b)
        {
            char[] value = { ToHexChar(b >> 4), ToHexChar(b) };
            return new string(value);
        }

        /// <summary>
        /// Convert an array into a string of hex digits.
        /// </summary>
        /// <param name="bytes">The bytes to convert</param>
        /// <returns>The converted string</returns>
        public static string ToHexString(byte[] bytes) => ToHexString(bytes, 0, bytes.Length);

        /// <summary>
        /// Convert a portion of an array into a string of hex digits.
        /// </summary>
        /// <param name="bytes">The bytes to convert</param>
        /// <param name="offset">The offset at which to start converting</param>
        /// <param name="length">The number of bytes to convert</param>
        /// <returns>The converted string</returns>
        public static string ToHexString(byte[] bytes, int offset, int length)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < length; i++)
                sb.Append(ToHexString(bytes[offset + i]));

            return sb.ToString();
        }

        /// <summary>
        /// Convert a string of hexadecimal digits to an array of bytes.
        /// If the string length is odd, a leading '0' is assumed.
        /// </summary>
        /// <param name="s">The string to convert to a byte array.</param>
        /// <returns>The byte array corresponding to the converted string</returns>
        public static byte[] FromHexString(string s)
        {
            var byteCount = (s.Length + 1) / 2;
            var bytes = new byte[byteCount];
            var index = 0;
            var offset = 0;

            // If an odd number of chars, assume leading zero
            if ((s.Length & 1) != 0)
                bytes[offset++] = FromHexNybble(s[index++]);

            while (index < s.Length)
            {
                var msn = FromHexNybble(s[index++]);
                var lsn = FromHexNybble(s[index++]);
                bytes[offset++] = (byte)((msn << 4) | lsn);
            }

            return bytes;
        }

        /// <summary>
        /// Convert an array of bytes into a Base-64 encoded using RFC 4648 rules.
        /// </summary>
        /// <param name="bytes">The bytes to encode</param>
        /// <returns>The base-64 encoded string</returns>
        public static string ToBase64String(byte[] bytes) => Convert.ToBase64String(bytes); // TODO : this might night to comply with https://tools.ietf.org/html/rfc4648

        /// <summary>
        /// Convert a base-64 encoded string into an array of bytes using RFC 4648 rules.
        /// </summary>
        /// <param name="s">The string to convert</param>
        /// <returns>The decoded bytes</returns>
        public static byte[] FromBase64String(string s) => Convert.FromBase64String(s);

        private static byte FromHexNybble(char value)
        {
            var c = char.ToLower(value);
            if (c >= '0' && c <= '9')
                return (byte)(c - '0');
            if (c >= 'a' && c <= 'f')
                return (byte)(10 + c - 'a');
            throw new ArgumentException("Unknown hex digit: " + value);
        }

        /// <summary>
        /// Trims any leading zero bytes from bytes until either no leading zero exists, or only a single zero byte exists.
        /// </summary>
        /// <param name="bytes">the byte[]</param>
        /// <returns>the bytes with leading zeros removed, if any</returns>
        public static byte[] TrimLeadingZeros(byte[] bytes)
        {
            if (bytes == null || bytes.Length <= 1 || bytes[0] != 0)
                return bytes;

            var trimLeadingZeros = 1;
            var maxTrim = bytes.Length - 1;

            while (trimLeadingZeros < maxTrim && bytes[trimLeadingZeros] == 0)
                trimLeadingZeros += 1;

            return Arrays.CopyOfRange(bytes, trimLeadingZeros, bytes.Length);
        }
    }
}