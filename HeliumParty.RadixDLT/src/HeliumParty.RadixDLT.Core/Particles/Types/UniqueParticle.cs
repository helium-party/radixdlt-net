using Dahomey.Cbor.Attributes;
using HeliumParty.RadixDLT.Identity;

namespace HeliumParty.RadixDLT.Particles.Types
{
    [CborDiscriminator("radix.particles.unique")]
    public class UniqueParticle : Particle, IIdentifiable
    {
        public RRI RRI => new RRI(Address, Name);
        public string Name { get; protected set; }
        public RadixAddress Address { get; protected set; }
        public long Nonce { get; protected set; }

        protected UniqueParticle() : base ()
        {

        }

        public UniqueParticle(RadixAddress address, string name)
            : base(address.EUID)
        {
            Name = name;
            Address = address;
            Nonce = RandomGenerator.GetRandomLong();
        }
    }
}