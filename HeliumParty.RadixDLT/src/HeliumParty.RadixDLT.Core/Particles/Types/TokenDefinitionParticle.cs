using Dahomey.Cbor.Attributes;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Primitives;
using Newtonsoft.Json;

namespace HeliumParty.RadixDLT.Particles.Types
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class TokenDefinitionParticle : Particle, IIdentifiable, IOwnable
    {
        [CborProperty("rri"), JsonProperty(PropertyName = "rri")]
        public RRI RRI { get; }
        
        public RadixAddress Address => RRI.Address;

        [CborProperty("name"), JsonProperty(PropertyName = "name")]
        public string Name { get; }

        [CborProperty("description"), JsonProperty(PropertyName = "description")]
        public string Description { get; }

        [CborProperty("granularity"), JsonProperty(PropertyName = "granularity")]
        public UInt256 Granularity { get; }

        [CborProperty("iconUrl"), JsonProperty(PropertyName = "iconUrl")]
        public string IconUrl { get; }

        [CborConstructor, JsonConstructor]
        protected TokenDefinitionParticle(RRI rRI, string name, string description, UInt256 granularity, string iconUrl)
            : base(rRI.Address.EUID)
        {
            RRI = rRI;
            Name = name;
            Description = description;
            Granularity = granularity;
            IconUrl = iconUrl;
        }
    }
}