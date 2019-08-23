using System;
using Dahomey.Cbor.Serialization;
using Dahomey.Cbor.Serialization.Converters;
using HeliumParty.RadixDLT.Primitives;

namespace HeliumParty.RadixDLT.Mapping
{
    public class DsonObjectConverter<T> : CborConverterBase<T>
    {
        private readonly byte _prefix;
        private readonly Func<T, byte[]> _convertTToBytes;
        private readonly Func<byte[], T> _convertBytesToT;

        public DsonObjectConverter(Func<T, byte[]> convertTToBytes, Func<byte[], T> convertBytesToT)
        {
            DsonCodecConstants.DsonPrefixesDictionary.TryGetValue(typeof(T), out _prefix);
            _convertTToBytes = convertTToBytes;
            _convertBytesToT = convertBytesToT;
        }

        public override T Read(ref CborReader reader)
        {
            var bytes = reader.ReadByteString().ToArray();
            if (bytes == null || bytes.Length == 0)
                throw new FormatException("Invalid input");
            if (bytes[0] != _prefix)
                throw new FormatException("Expected prefix " + Bytes.ToHexString(_prefix) + " but got " + Bytes.ToHexString(bytes[0]));

            return _convertBytesToT(Arrays.CopyOfRange(bytes, 1, bytes.Length));
        }

        public override void Write(ref CborWriter writer, T value)
        {
            var valueBytes = _convertTToBytes(value);
            var bytes = new byte[1 + valueBytes.Length];

            bytes[0] = _prefix;
            Array.Copy(valueBytes, 0, bytes, 1, valueBytes.Length);
            writer.WriteByteString(bytes);
        }
    }
}