using Dahomey.Cbor.Attributes;
using HeliumParty.RadixDLT.Atoms;

namespace HeliumParty.RadixDLT.Particles
{
    [CborDiscriminator("radix.spun_particle", Policy =CborDiscriminatorPolicy.Always)]
    public class SpunParticle : SerializableObject
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
    }
}