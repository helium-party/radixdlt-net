using Dahomey.Cbor.Attributes;

namespace HeliumParty.RadixDLT.Particles
{
    [CborDiscriminator("radix.spun_particle")]
    public class SpunParticle<T> where T : Particle
    {
        public Spin Spin { get; protected set; }
        public T Particle { get; protected set; }

        public SpunParticle()
        {

        }

        public SpunParticle(T particle, Spin spin)            
        {
            Particle = particle;
            Spin = spin;
        }
    }
}