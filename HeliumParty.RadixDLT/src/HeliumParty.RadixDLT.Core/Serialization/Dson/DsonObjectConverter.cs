using Dahomey.Cbor.Serialization;
using Dahomey.Cbor.Serialization.Converters;
using HeliumParty.RadixDLT.Primitives;
using System;
using HeliumParty.RadixDLT.EllipticCurve;

namespace HeliumParty.RadixDLT.Serialization.Dson
{
    public class DsonObjectConverter<T> : CborConverterBase<T>
    {        
        private readonly Func<T, byte[]> _convertTToBytes;
        private readonly Func<byte[], T> _convertBytesToT;

        private bool _usePrefix = false;
        private byte _prefix = 0x00;

        public DsonObjectConverter(Func<T, byte[]> convertTToBytes, Func<byte[], T> convertBytesToT)
        {
            _convertTToBytes = convertTToBytes;
            _convertBytesToT = convertBytesToT;
            SetPrefix();
        }

        /// <summary>
        ///     set usePrefix and prefix correctly based on <typeparam>T</typeparam>
        /// </summary>
        protected virtual void SetPrefix()
        {
            var attrs = typeof(T).GetCustomAttributes(typeof(SerializationPrefixAttribute), true);

            if (attrs.Length == 0)
            {
                PrimitivePrefixes(typeof(T));
            }
            else
            {
                var attr = (SerializationPrefixAttribute)attrs[0];
                _usePrefix = attr.HasDsonPrefix;

                _prefix = attr.Dson;
            }
        }

        /// <summary>
        ///     set a prefix for certain primitive types
        /// </summary>
        /// <param name="type"></param>
        private void PrimitivePrefixes(Type type)
        {
            if (type == typeof(byte[]) || type == typeof(ECPrivateKey) || type == typeof(ECPublicKey))
            {
                _prefix = 0x01;
                _usePrefix = true;
            }

            if (type == typeof(UInt256))
            {
                _prefix = 0x05;
                _usePrefix = true;
            }
        }

        public override T Read(ref CborReader reader)
        {            
            var bytes = reader.ReadByteString().ToArray();
            if (bytes == null || bytes.Length == 0)
                throw new FormatException("Invalid input");

            if (_usePrefix)
            {
                if (bytes[0] != _prefix)
                    throw new FormatException($"Expected prefix 0x{Bytes.ToHexString(_prefix)} but is 0x{Bytes.ToHexString(bytes[0])}");

                return _convertBytesToT(Arrays.CopyOfRange(bytes, 1, bytes.Length));
            }
            else return _convertBytesToT(bytes);
        }

        public override void Write(ref CborWriter writer, T value)
        {
            var valueBytes = _convertTToBytes(value);
            byte[] bytes = null;

            if (_usePrefix)
            {
                bytes = new byte[1 + valueBytes.Length];
                bytes[0] = _prefix;
                Array.Copy(valueBytes, 0, bytes, 1, valueBytes.Length);

            }
            else bytes = valueBytes;

            writer.WriteByteString(bytes);
        }
    }
}
