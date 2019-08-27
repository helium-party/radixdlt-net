using System.Collections.Generic;
using Dahomey.Cbor.Attributes;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Primitives;
using Newtonsoft.Json;

namespace HeliumParty.RadixDLT.Particles.Types
{
    /// <summary>
    /// A particle which represents an amount of consumable and consuming, tranferable fungible tokens
    /// owned by some key owner and stored in an account.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class TransferableTokensParticle : Particle, IAccountable, IOwnable
    {
        [CborProperty("address"), JsonProperty(PropertyName = "address")]
        public RadixAddress Address { get; }

        public HashSet<RadixAddress> Addresses => new HashSet<RadixAddress>() { Address };

        [CborProperty("tokenDefinitionReference"), JsonProperty(PropertyName = "tokenDefinitionReference")]
        public RRI TokenDefinitionReference { get; }

        [CborProperty("granularity"), JsonProperty(PropertyName = "granularity")]
        public UInt256 Granularity { get; }

        [CborProperty("planck"), JsonProperty(PropertyName = "planck")]
        public long Planck { get; }

        [CborProperty("nonce"), JsonProperty(PropertyName = "nonce")]
        public long Nonce { get; }

        [CborProperty("amount"), JsonProperty(PropertyName = "amount")]
        public UInt256 Amount { get; set; }

        [CborProperty("permissions"), JsonProperty(PropertyName = "permissions")]
        public IDictionary<TokenTransition, TokenPermission> TokenPermissions { get; }

        [CborConstructor, JsonConstructor]
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