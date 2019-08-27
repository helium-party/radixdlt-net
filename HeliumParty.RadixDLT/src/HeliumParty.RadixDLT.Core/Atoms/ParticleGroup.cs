using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Dahomey.Cbor.Attributes;
using HeliumParty.RadixDLT.Particles;
using Newtonsoft.Json;

namespace HeliumParty.RadixDLT.Atoms
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ParticleGroup
    {
        [CborProperty("particles"), JsonProperty(PropertyName = "particles")]
        public ImmutableList<SpunParticle<Particle>> Particles { get; }

        [CborProperty("metaData"), JsonProperty(PropertyName = "metaData")]
        public ImmutableDictionary<string, string> MetaData { get; }

        [CborConstructor, JsonConstructor]
        public ParticleGroup(ImmutableList<SpunParticle<Particle>> particles, ImmutableDictionary<string, string> metaData)
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