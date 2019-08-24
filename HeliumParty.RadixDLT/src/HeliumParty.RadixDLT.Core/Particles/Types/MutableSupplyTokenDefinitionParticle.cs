using System.Collections.Generic;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Primitives;

namespace HeliumParty.RadixDLT.Particles.Types
{
    public class MutableSupplyTokenDefinitionParticle : TokenDefinitionParticle
    {
        public string Symbol { get; private set; }
        public IDictionary<TokenTransition, TokenPermission> TokenPermissions { get; private set; }

        public MutableSupplyTokenDefinitionParticle(RRI rRI, string name, string description, UInt256 granularity, string iconUrl, string symbol, IDictionary<TokenTransition, TokenPermission> tokenPermissions)
            : base(rRI, name, description, granularity, iconUrl)
        {
            Symbol = symbol;
            TokenPermissions = tokenPermissions;
        }
    }
}