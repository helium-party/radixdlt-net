using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using HeliumParty.RadixDLT.Particles;

namespace HeliumParty.RadixDLT.Atoms
{
    public class ParticleGroup<T> where T : Particle
    {
        public ImmutableList<SpunParticle<T>> Particles { get; }
        public ImmutableDictionary<string, string> MetaData { get; }

        public ParticleGroup(ImmutableList<SpunParticle<T>> particles, ImmutableDictionary<string, string> metaData)
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