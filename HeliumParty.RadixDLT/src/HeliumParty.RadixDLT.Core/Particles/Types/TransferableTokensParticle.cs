using System.Collections.Generic;
using Dahomey.Cbor.Attributes;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Primitives;

namespace HeliumParty.RadixDLT.Particles.Types
{
    /// <summary>
    /// A particle which represents an amount of consumable and consuming, tranferable fungible tokens
    /// owned by some key owner and stored in an account.
    /// </summary>
    [CborDiscriminator("radix.particles.transferrable_tokens")]
    public class TransferableTokensParticle : Particle, IAccountable, IOwnable
    {
        public RadixAddress Address { get; protected set; }
        public HashSet<RadixAddress> Addresses => new HashSet<RadixAddress>() { Address };
        public RRI TokenDefinitionReference { get; protected set; }
        //public UInt256 Granularity { get; protected set; }
        public long Planck { get; protected set; }
        public long Nonce { get; protected set; }
        //public UInt256 Amount { get; protected set; }
        public IDictionary<TokenTransition, TokenPermission> TokenPermissions { get; protected set; }

        protected TransferableTokensParticle() : base () { }

        public TransferableTokensParticle(
            RadixAddress address, 
            RRI tokenDefinitionReference, 
            UInt256 granularity, 
            long planck, long nonce, UInt256 amount, 
            IDictionary<TokenTransition, TokenPermission> tokenPermissions,
            EUID destination //origin of address property
            )
            : base(destination)
        {
            Address = address;
            TokenDefinitionReference = tokenDefinitionReference;
            //Granularity = granularity;
            Planck = planck;
            Nonce = nonce;
            //Amount = amount;
            TokenPermissions = tokenPermissions;
        }
    }
}