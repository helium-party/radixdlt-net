using Dahomey.Cbor.Attributes;
using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.Serialization;

namespace HeliumParty.RadixDLT.Jsonrpc
{
    [CborDiscriminator("network.peer", Policy = CborDiscriminatorPolicy.Always)]
    public class NodeRunnerData : SerializableObject
    {
        [SerializationPrefix(Json = "system")]
        [SerializationOutput(OutputMode.All)]
        private RadixSystem _System;

        [SerializationPrefix(Json = "host")]
        [SerializationOutput(OutputMode.All)]
        public string IP { get; private set; }
        public ShardSpace Shards => _System.Shards;

        public NodeRunnerData(RadixSystem system)
        {
            IP = null;
            _System = system;
        }

        public override string ToString() => $"{(IP != null ? IP + ":" : "")}shards={Shards}";

        public override int GetHashCode() => (IP + Shards.ToString()).GetHashCode();  // TODO: Java lib might change here

        public override bool Equals(object obj)
        {
            if (obj != null && obj is NodeRunnerData nrd)
                return nrd.IP.Equals(this.IP) && nrd.Shards.Equals(this.Shards);

            return false;
        }
    }
}
