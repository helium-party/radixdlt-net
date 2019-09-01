using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT.Jsonrpc
{
    public class RadixSystem : Radix.Serialization.Client.SerializableObject
    {
        private ShardSpace _Shards;
        public ShardSpace GetShards() => _Shards;
    }
}
