using Dahomey.Cbor.Attributes;
using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.Serialization;

namespace HeliumParty.RadixDLT.Jsonrpc
{
    [CborDiscriminator("api.system", Policy = CborDiscriminatorPolicy.Always)]
    public class RadixSystem : SerializableObject
    {
        [SerializationOutput(OutputMode.All)]
        public ShardSpace Shards { get; }
    }
}
