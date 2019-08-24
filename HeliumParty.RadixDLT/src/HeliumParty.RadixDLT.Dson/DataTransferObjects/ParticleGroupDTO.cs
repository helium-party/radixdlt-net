using System.Collections.Generic;
using HeliumParty.RadixDLT.Particles;
using Org.BouncyCastle.Asn1.Mozilla;

namespace HeliumParty.RadixDLT.DataTransferObjects
{
    public class ParticleGroupDTO<T> where T : ParticleDTO
    {
        public List<SpunParticleDTO<T>> Particles { get; set; }

        public Dictionary<string, string> MetaData { get; set; }
    }
}