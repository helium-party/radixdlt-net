using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Primitives;

namespace HeliumParty.RadixDLT.Particles.Types
{
    public class FixedSupplyTokenDefinitionParticle : TokenDefinitionParticle
    {
        public UInt256 Supply { get; }

        public FixedSupplyTokenDefinitionParticle(RRI rRI, string name, string description, UInt256 granularity, string iconUrl, UInt256 supply)
            : base(rRI, name, description, granularity, iconUrl)
        {
            Supply = supply;
        }
    }
}