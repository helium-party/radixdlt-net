using HeliumParty.RadixDLT.Identity;

namespace HeliumParty.RadixDLT.Particles.Types
{
    public class UniqueParticle : Particle, IIdentifiable
    {
        public RRI RRI => new RRI(Address, Name);
        public string Name { get; protected set; }
        public RadixAddress Address { get; protected set; }
        public long Nonce { get; protected set; }

        public UniqueParticle()
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