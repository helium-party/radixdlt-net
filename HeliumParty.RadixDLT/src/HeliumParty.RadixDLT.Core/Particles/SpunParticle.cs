using Dahomey.Cbor.Attributes;
using Newtonsoft.Json;

namespace HeliumParty.RadixDLT.Particles
{
    public class SpunParticle<T> where T : Particle
    {
        [CborProperty("spin"), JsonProperty(PropertyName = "spin")]
        public Spin Spin { get; }

        [CborProperty("particle"), JsonProperty(PropertyName = "particle")]
        public T Particle { get; }

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