using System.Collections.Generic;
using Dahomey.Cbor.Attributes;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Primitives;

namespace HeliumParty.RadixDLT.Particles.Types
{
    /// <summary>
    /// A particle which represents an amount of unallocated tokens which can be minted.
    /// </summary>
    [CborDiscriminator("radix.particles.unallocated_tokens", Policy = CborDiscriminatorPolicy.Always)]
    public class UnallocatedTokensParticle : Particle, IAccountable, IOwnable
    {
        public HashSet<RadixAddress> Addresses => new HashSet<RadixAddress>() { Address };
        public RadixAddress Address => TokenDefinitionReference.Address;

        public RRI TokenDefinitionReference { get; protected set; }
        public UInt256 Granularity { get; protected set; }
        public long Nonce { get; protected set; }
        public UInt256 Amount { get; protected set; }
        public IDictionary<TokenTransition, TokenPermission> TokenPermissions { get; protected set; }

        protected UnallocatedTokensParticle() : base () { }

        public UnallocatedTokensParticle(
            RRI tokenDefinitionReference, 
            UInt256 granularity, 
            long nonce, 
            UInt256 amount, 
            IDictionary<TokenTransition, TokenPermission> tokenPermissions,
            EUID destination // originates from rri property
            ): base(destination)
        {
            TokenDefinitionReference = tokenDefinitionReference;
            Granularity = granularity;
            Nonce = nonce;
            Amount = amount;
            TokenPermissions = tokenPermissions;
        }
    }
}