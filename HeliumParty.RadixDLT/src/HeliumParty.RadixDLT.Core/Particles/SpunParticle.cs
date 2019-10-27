using Dahomey.Cbor.Attributes;

namespace HeliumParty.RadixDLT.Particles
{
    [CborDiscriminator("radix.spun_particle",Policy =CborDiscriminatorPolicy.Always)]
    public class SpunParticle//<T> where T : Particle
    {
        public Spin Spin { get; protected set; }
        public Particle Particle { get; protected set; }

        public SpunParticle()
        {

        }

        public SpunParticle(Particle particle, Spin spin)            
        {
            Particle = particle;
            Spin = spin;
        }

        public static SpunParticle Up (Particle p)
        {
            return new SpunParticle(p, Spin.Up);
        }

        public static SpunParticle Down(Particle p)
        {
            return new SpunParticle(p, Spin.Down);
        }
    }
}