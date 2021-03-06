﻿using System;
using System.Linq;
using HeliumParty.RadixDLT.Primitives;
using HeliumParty.RadixDLT.Serialization;

namespace HeliumParty.RadixDLT.Atoms
{
    /// <summary>
    /// An Atom ID, made up of 192 bits of truncated hash and 64 bits of a selected shard.    
    /// The Atom ID is used so that Atoms can be located using just their hid.
    /// </summary>
    [SerializationPrefix(Json = ":aid:" , Dson = 0x08)]
    public class AID
    {
        public const int HashBytesSize = 24;
        public const int ShardBytesSize = 8;

        public byte[] Bytes { get; } = new byte[HashBytesSize + ShardBytesSize];

        public AID(byte[] bytes)
        {
            if (bytes.Length != HashBytesSize + ShardBytesSize)
                throw new ArgumentException($"{nameof(bytes)} should be length {HashBytesSize + ShardBytesSize} but was {bytes.Length}");
            Array.Copy(bytes, Bytes, Bytes.Length);
        }

        public AID(byte[] hashBytes, byte[] shardBytes)
        {
            if (hashBytes.Length != HashBytesSize)
                throw new ArgumentException($"{nameof(hashBytes)} should be length {HashBytesSize} but was {hashBytes.Length}");
            if (shardBytes.Length != ShardBytesSize)
                throw new ArgumentException($"{nameof(shardBytes)} should be length {ShardBytesSize} but was {shardBytes.Length}");

            Bytes = Arrays.ConcatArrays(hashBytes, shardBytes);
        }

        public AID(string hexBytes) : this(Primitives.Bytes.FromHexString(hexBytes)) { }

        public long Shard => Longs.FromByteArray(Bytes, HashBytesSize);

        public override bool Equals(object obj)
        {
            if (obj == this)
                return true;

            if (!(obj.GetType() == typeof(AID)))
                return false;

            return Bytes.SequenceEqual(((AID)obj).Bytes);
        }

        public override string ToString() => Primitives.Bytes.ToHexString(Bytes);
    }
}