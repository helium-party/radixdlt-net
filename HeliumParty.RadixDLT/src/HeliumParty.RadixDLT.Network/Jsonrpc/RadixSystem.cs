using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT.Jsonrpc
{
    public class RadixSystem : Radix.Serialization.Client.SerializableObject
    {
        private ShardSpace _Shards;
        ShardSpace GetShards() => _Shards;
    }
}
