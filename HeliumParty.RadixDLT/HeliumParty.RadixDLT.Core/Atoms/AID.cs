using System;
using HeliumParty.RadixDLT.Primitives;

namespace HeliumParty.RadixDLT.Atoms
{
    /// <summary>
    /// An Atom ID, made up of 192 bits of truncated hash and 64 bits of a selected shard.    
    /// The Atom ID is used so that Atoms can be located using just their hid.
    /// </summary>
    public class AID
    {
        public const int HashBytesSize = 24;
        public const int ShardBytesSize = 8;

        private readonly byte[] _bytes = new byte[HashBytesSize + ShardBytesSize];

        public AID(byte[] bytes)
        {
            if (bytes.Length != HashBytesSize + ShardBytesSize)
                throw new ArgumentException($"{nameof(bytes)} should be length {HashBytesSize + ShardBytesSize} but was {bytes.Length}");
            Array.Copy(bytes, _bytes, _bytes.Length);
        }

        public AID(byte[] hashBytes, byte[] shardBytes)
        {
            if (hashBytes.Length != HashBytesSize)
                throw new ArgumentException($"{nameof(hashBytes)} should be length {HashBytesSize} but was {hashBytes.Length}");
            if (shardBytes.Length != ShardBytesSize)
                throw new ArgumentException($"{nameof(shardBytes)} should be length {ShardBytesSize} but was {shardBytes.Length}");

            _bytes = Arrays.ConcatArrays(hashBytes, shardBytes);
        }

        public AID(string hexBytes) : this(Bytes.FromHexString(hexBytes)) { }

        public long Shard => Longs.FromByteArray(_bytes, HashBytesSize);

        public override string ToString() => Bytes.ToHexString(_bytes);
    }
}