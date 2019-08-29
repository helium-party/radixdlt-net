using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT.Jsonrpc
{
    public class ShardSpace
    {
        public const int ShardChunks = 1 << 20;
        public const long ShardChunkRange = -(long.MinValue / ShardChunks) * 2;
        public const long ShardChunkHalfRange = -(long.MinValue / ShardChunks);

        public static readonly ShardRange ShardRangeFull = new ShardRange(-ShardChunkHalfRange, ShardChunkHalfRange - 1);


    }
}
