namespace HeliumParty.RadixDLT.Primitives
{
    /// <summary>
    /// Some useful string handling methods, currently mostly here for performance reasons.
    /// </summary>
    public static class Strings
    {
        /// <summary>
        /// Convert a string to a sequence of ASCII bytes by discarding all but the lower 7 bits of each char
        /// </summary>
        /// <param name="s">The string to convert</param>
        /// <param name="offset">The offset within the buffer to place the converted bytes</param>
        /// <returns>The byte array</returns>
        public static byte[] ToAsciiBytes(string s, int offset)
        {
            var bytes = new byte[s.Length];
            for (int i = 0; i < s.Length; i++)
                bytes[offset + i] = (byte)(s[i] & 0x7F);

            return bytes;
        }

        /// <summary>
        /// Convert a sequence of ASCII bytes into a string.
        /// </summary>
        /// <param name="bytes">The byte array to convert to a string.</param>
        /// <param name="offset">The offset within the byte array</param>
        /// <param name="length">The number of bytes to convert</param>
        /// <returns>The string</returns>
        public static string ToString(byte[] bytes, int offset, int length)
        {
            var chars = new char[length];
            for (int i = 0; i < length; i++)
                chars[i] = (char)(bytes[offset + i] & 0x7F);

            return new string(chars);
        }
    }
}