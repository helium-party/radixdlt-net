﻿using Dahomey.Cbor.Attributes;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Primitives;

namespace HeliumParty.RadixDLT.Particles.Types
{
    [CborDiscriminator("radix.particles.fixed_supply_token_definition", Policy = CborDiscriminatorPolicy.Always)]
    public class FixedSupplyTokenDefinitionParticle : Particle, IIdentifiable, IOwnable
    {
        public RRI RRI { get; protected set; }
        public RadixAddress Address => RRI.Address;
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public UInt256 Granularity { get; protected set; }
        public string IconUrl { get; protected set; }
        public UInt256 Supply { get; protected set; }

        protected FixedSupplyTokenDefinitionParticle() : base()
        {

        }

        public FixedSupplyTokenDefinitionParticle(RRI rRI, string name, string description, UInt256 granularity, string iconUrl, UInt256 supply)            
        {
            RRI = rRI;
            Name = name;
            Description = description;
            Granularity = granularity;
            IconUrl = iconUrl;
            Supply = supply;
        }
    }
}