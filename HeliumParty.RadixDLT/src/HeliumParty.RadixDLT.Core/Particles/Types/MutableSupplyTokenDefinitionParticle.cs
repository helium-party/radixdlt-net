using System.Collections.Generic;
using Dahomey.Cbor.Attributes;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Primitives;

namespace HeliumParty.RadixDLT.Particles.Types
{
    [CborDiscriminator("radix.particles.mutable_supply_token_definition")]
    public class MutableSupplyTokenDefinitionParticle : Particle//, IIdentifiable, IOwnable
    {
        public RRI RRI { get; protected set; }
        public RadixAddress Address => RRI.Address;
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        //public UInt256 Granularity { get; protected set; }
        public string IconUrl { get; protected set; }
        public string Symbol { get; protected set; }
        public IDictionary<TokenTransition, TokenPermission> TokenPermissions { get; protected set; }

        protected MutableSupplyTokenDefinitionParticle() : base() { }
        public MutableSupplyTokenDefinitionParticle(RRI rRI, string name, string description, UInt256 granularity, string iconUrl, string symbol, IDictionary<TokenTransition, TokenPermission> tokenPermissions)            
        {
            RRI = rRI;
            Name = name;
            Description = description;
            //Granularity = granularity;
            IconUrl = iconUrl;
            Symbol = symbol;
            TokenPermissions = tokenPermissions;
        }
    }
}