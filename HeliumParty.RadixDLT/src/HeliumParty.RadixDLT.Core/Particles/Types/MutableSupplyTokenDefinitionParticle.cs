using System.Collections.Generic;
using Dahomey.Cbor.Attributes;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Primitives;
using Newtonsoft.Json;

namespace HeliumParty.RadixDLT.Particles.Types
{
    public class MutableSupplyTokenDefinitionParticle : TokenDefinitionParticle
    {
        [CborProperty("permissions"), JsonProperty(PropertyName = "permissions")]
        public IDictionary<TokenTransition, TokenPermission> TokenPermissions { get; private set; }

        public MutableSupplyTokenDefinitionParticle(RRI rRI, string name, string description, UInt256 granularity, string iconUrl, IDictionary<TokenTransition, TokenPermission> tokenPermissions)
            : base(rRI, name, description, granularity, iconUrl)
        {
            TokenPermissions = tokenPermissions;
        }
    }
}