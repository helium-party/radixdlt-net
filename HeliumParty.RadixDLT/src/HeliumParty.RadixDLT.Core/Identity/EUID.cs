using System;
using System.Globalization;
using HeliumParty.RadixDLT.Primitives;
using HeliumParty.RadixDLT.Serialization;

namespace HeliumParty.RadixDLT.Identity
{
    [SerializationPrefix(Json = ":uid:", Dson = 0x02)]
    public class EUID : IComparable<EUID>
    {
        // TODO add methods like RingClosest,... from former HeliumParty lib? (this comment is here as a remainder that these methods have already been written
        public static readonly EUID Zero = new EUID(0);
        public static readonly EUID One = new EUID(1);
        public static readonly EUID Two = new EUID(2);

        public const int Bytes = 16;

        private readonly Int128 _value;

        public EUID(long v)
        {
            _value = new Int128(v);
        }

        /// <summary>
        /// convert a hex representation of a int128 to int128
        /// </summary>
        public EUID(string s)
        {
            Int128.TryParse(s, NumberStyles.HexNumber, NumberFormatInfo.CurrentInfo, out _value);
        }

        public EUID(Int128 n)
        {
            _value = n;
        }

        public EUID(byte[] bytes)
        {
            var newBytes = Extend(bytes);
            var high = Longs.FromByteArray(newBytes, 0);
            var low = Longs.FromByteArray(newBytes, 8);

            _value = Int128.Create(low, high);
        }

        private static byte[] Extend(byte[] bytes)
        {
            if (bytes.Length >= Bytes)
            {
                return bytes;
            }

            var newBytes = new byte[Bytes];
            var newPos = Bytes - bytes.Length;
            // Sign extension            
            //newBytes.Fill(0, newPos, (bytes[0] < 0) ? (byte)0xFF : (byte)0x00);
            Arrays.Fill(ref newBytes, 0, newPos - 1, (byte)0x00);
            Array.Copy(bytes, 0, newBytes, newPos, bytes.Length);
            return newBytes;
        }

        public long Shard => _value.High;

        public long Low => _value.Low;

        public byte[] ToByteArray() => _value.ToByteArray();

        public override string ToString()
        {
            return _value.ToString("x2");
        }

        public override int GetHashCode() => _value.GetHashCode();

        public override bool Equals(object obj)
        {
            if (obj == this)
                return true;
            if (!(obj is EUID))
                return false;

            var other = (EUID)obj;
            return _value.Equals(other._value);
        }

        public int CompareTo(EUID other) => _value.CompareToUnsigned(other._value);
    }
}