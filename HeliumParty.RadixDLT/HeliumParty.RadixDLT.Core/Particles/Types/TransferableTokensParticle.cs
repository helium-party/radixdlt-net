using System.Collections.Generic;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Primitives;

namespace HeliumParty.RadixDLT.Particles.Types
{
    /// <summary>
    /// A particle which represents an amount of consumable and consuming, tranferable fungible tokens
    /// owned by some key owner and stored in an account.
    /// </summary>
    public class TransferableTokensParticle : Particle, IAccountable, IOwnable
    {
        public RadixAddress Address { get; }
        public HashSet<RadixAddress> Addresses => new HashSet<RadixAddress>() { Address };
        public RRI TokenDefinitionReference { get; }
        public UInt256 Granularity { get; }
        public long Planck { get; }
        public long Nonce { get; }
        public UInt256 Amount { get; set; }
        public IDictionary<TokenTransition, TokenPermission> TokenPermissions { get; }

        public TransferableTokensParticle(RadixAddress address, RRI tokenDefinitionReference, UInt256 granularity, long planck, long nonce, UInt256 amount, IDictionary<TokenTransition, TokenPermission> tokenPermissions)
            : base(address.EUID)
        {
            Address = address;
            TokenDefinitionReference = tokenDefinitionReference;
            Granularity = granularity;
            Planck = planck;
            Nonce = nonce;
            Amount = amount;
            TokenPermissions = tokenPermissions;
        }
    }
}