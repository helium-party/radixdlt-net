using System;
using System.Numerics;

namespace HeliumParty.RadixDLT.Primitives
{
    public struct UInt256
    {
        public ulong s0;
        public ulong s1;
        public ulong s2;
        public ulong s3;

        public uint r0 => (uint)s0;
        public uint r1 => (uint)(s0 >> 32);
        public uint r2 => (uint)s1;
        public uint r3 => (uint)(s1 >> 32);
        public uint r4 => (uint)s2;
        public uint r5 => (uint)(s2 >> 32);
        public uint r6 => (uint)s3;
        public uint r7 => (uint)(s3 >> 32);

        public UInt128 t0 { get { UInt128 result; UInt128.Create(out result, s0, s1); return result; } }
        public UInt128 t1 { get { UInt128 result; UInt128.Create(out result, s2, s3); return result; } }

        public static implicit operator BigInteger(UInt256 a)
        {
            return (BigInteger)a.s3 << 192 | (BigInteger)a.s2 << 128 | (BigInteger)a.s1 << 64 | a.s0;
        }

        public static implicit operator UInt256(byte[] bigint)
        {
            if (bigint.Length > 32)
                throw new Exception($"cannot parse byte[] to UInt256. array was to big : length {bigint.Length}");

            var b = new UInt256();

            if (bigint.Length < 2)
                b.s0 = bigint[0];
            else if (bigint.Length < 4)
                b.s0 = BitConverter.ToUInt16(bigint, 0);
            else if (bigint.Length < 8)
                b.s0 = BitConverter.ToUInt32(bigint, 0);
            else
                b.s0 = BitConverter.ToUInt64(bigint, 0);

            if (bigint.Length >= 16)
                b.s1 = BitConverter.ToUInt64(bigint, 8);
            else b.s1 = 0;

            if (bigint.Length >= 24)
                b.s2 = BitConverter.ToUInt64(bigint, 16);
            else b.s2 = 0;

            if (bigint.Length == 32)
                b.s3 = BitConverter.ToUInt64(bigint, 24);
            else b.s3 = 0;

            return b;
        }

        public static implicit operator UInt256(BigInteger bigint)
        {
            return bigint.ToByteArray();
        }

        public static implicit operator UInt256(BigDecimal bigdec)
        {
            return (BigInteger)bigdec;
        }

        public static implicit operator byte[](UInt256 b)
        {
            var b1 = BitConverter.GetBytes(b.s0);
            var b2 = BitConverter.GetBytes(b.s1);
            var b3 = BitConverter.GetBytes(b.s2);
            var b4 = BitConverter.GetBytes(b.s3);

            return Arrays.ConcatArrays(b1, b2, b3, b4);            
        }

        public override string ToString()
        {
            return ((BigInteger)this).ToString();
        }
    }
}