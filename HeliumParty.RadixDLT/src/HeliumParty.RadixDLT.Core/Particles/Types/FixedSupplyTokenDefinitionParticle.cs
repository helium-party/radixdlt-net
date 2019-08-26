using Dahomey.Cbor.Attributes;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Primitives;
using Newtonsoft.Json;

namespace HeliumParty.RadixDLT.Particles.Types
{
    public class FixedSupplyTokenDefinitionParticle : TokenDefinitionParticle
    {
        [CborProperty("supply"), JsonProperty(PropertyName = "supply")]
        public UInt256 Supply { get; }

        public FixedSupplyTokenDefinitionParticle(RRI rRI, string name, string description, UInt256 granularity, string iconUrl, UInt256 supply)
            : base(rRI, name, description, granularity, iconUrl)
        {
            Supply = supply;
        }
    }
}