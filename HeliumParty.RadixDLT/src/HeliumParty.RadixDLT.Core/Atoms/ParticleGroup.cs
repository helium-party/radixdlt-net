using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Dahomey.Cbor.Attributes;
using HeliumParty.RadixDLT.Particles;

namespace HeliumParty.RadixDLT.Atoms
{
    [CborDiscriminator("radix.particle_group", Policy = CborDiscriminatorPolicy.Always)]
    public class ParticleGroup : SerializableObject
    {           
        public ImmutableList<SpunParticle> Particles { get; protected set; }
        public ImmutableDictionary<string, string> MetaData { get; protected set; }

        public bool ShouldSerializeMetaData()
        {
            if (MetaData == null) return false;
            if (MetaData.IsEmpty) return false;
            return true;
        }

        public ParticleGroup() { }

        public ParticleGroup(ImmutableList<SpunParticle> particles, ImmutableDictionary<string, string> metaData)
        {
            Particles = particles;
            MetaData = metaData;
        }

        public IEnumerable<Particle> GetParticlesWithSpin(Spin spin)
        {
            return Particles.Where(p => p.Spin == spin).Select(p => p.Particle);
        }
    }
}