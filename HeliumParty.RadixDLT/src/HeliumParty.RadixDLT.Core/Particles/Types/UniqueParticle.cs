using Dahomey.Cbor.Attributes;
using HeliumParty.RadixDLT.Identity;
using Newtonsoft.Json;

namespace HeliumParty.RadixDLT.Particles.Types
{
    public class UniqueParticle : Particle, IIdentifiable
    {
        // TODO check if this rri should be serialized
        public RRI RRI => new RRI(Address, Name);

        [CborProperty("name"), JsonProperty(PropertyName = "name")]
        public string Name { get; }

        [CborProperty("address"), JsonProperty(PropertyName = "address")]
        public RadixAddress Address { get; }

        [CborProperty("nonce"), JsonProperty(PropertyName = "nonce")]
        public long Nonce { get; }

        public UniqueParticle(RadixAddress address, string name)
            : base(address.EUID)
        {
            Name = name;
            Address = address;
            Nonce = RandomGenerator.GetRandomLong();
        }
    }
}