using System.Collections.Generic;
using Dahomey.Cbor.Attributes;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Primitives;
using Newtonsoft.Json;

namespace HeliumParty.RadixDLT.Particles.Types
{
    /// <summary>
    /// A particle which represents an amount of unallocated tokens which can be minted.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class UnallocatedTokensParticle : Particle, IAccountable, IOwnable
    {
        public HashSet<RadixAddress> Addresses => new HashSet<RadixAddress>() { Address };
        public RadixAddress Address => TokenDefinitionReference.Address;

        [CborProperty("tokenDefinitionReference"), JsonProperty(PropertyName = "tokenDefinitionReference")]
        public RRI TokenDefinitionReference { get; }

        [CborProperty("granularity"), JsonProperty(PropertyName = "granularity")]
        public UInt256 Granularity { get; }

        [CborProperty("nonce"), JsonProperty(PropertyName = "nonce")]
        public long Nonce { get; }

        [CborProperty("amount"), JsonProperty(PropertyName = "amount")]
        public UInt256 Amount { get; }

        [CborProperty("permissions"), JsonProperty(PropertyName = "permissions")]
        public IDictionary<TokenTransition, TokenPermission> TokenPermissions { get; }

        [CborConstructor, JsonConstructor]
        public UnallocatedTokensParticle(RRI tokenDefinitionReference, UInt256 granularity, long nonce, UInt256 amount, IDictionary<TokenTransition, TokenPermission> tokenPermissions)
            : base(tokenDefinitionReference.Address.EUID)
        {
            TokenDefinitionReference = tokenDefinitionReference;
            Granularity = granularity;
            Nonce = nonce;
            Amount = amount;
            TokenPermissions = tokenPermissions;
        }
    }
}