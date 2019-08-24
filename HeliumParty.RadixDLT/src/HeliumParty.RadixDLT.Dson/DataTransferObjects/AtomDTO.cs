using System.Collections.Generic;
using HeliumParty.RadixDLT.Atoms;

namespace HeliumParty.RadixDLT.DataTransferObjects
{
    public class AtomDTO
    {
        public List<ParticleGroupDTO<ParticleDTO>> ParticleGroups { get; set; }

        public Dictionary<string, ECSignatureDTO> Signatures { get; set; }

        public Dictionary<string, string> MetaData { get; set; }

        public AID Id { get; set; }
    }
}