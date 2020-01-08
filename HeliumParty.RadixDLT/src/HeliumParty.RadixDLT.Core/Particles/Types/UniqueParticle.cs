using Dahomey.Cbor.Attributes;
using HeliumParty.RadixDLT.Identity;
using Newtonsoft.Json;

namespace HeliumParty.RadixDLT.Particles.Types
{
    [CborDiscriminator("radix.particles.unique", Policy = CborDiscriminatorPolicy.Always)]
    [JsonObject(ItemTypeNameHandling = TypeNameHandling.None)]
    public class UniqueParticle : Particle, IIdentifiable
    {
        public RRI RRI => new RRI(Address, Name);
        public string Name { get; protected set; }
        public RadixAddress Address { get; protected set; }
        public long Nonce { get; protected set; }

        protected UniqueParticle() : base ()
        {

        }

        public UniqueParticle(
            RadixAddress address, 
            string name,
            EUID destination// originates from address
            )
            : base(destination)
        {
            Name = name;
            Address = address;
            Nonce = RandomGenerator.GetRandomLong();
        }
    }
}