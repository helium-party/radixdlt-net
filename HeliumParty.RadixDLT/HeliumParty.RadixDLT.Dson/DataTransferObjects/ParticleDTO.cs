using System.Collections.Generic;
using HeliumParty.RadixDLT.Identity;

namespace HeliumParty.RadixDLT.DataTransferObjects
{
    // TODO do we need this? particle is abstract
    public abstract class ParticleDTO
    {
        public HashSet<EUID> Destinations { get; set; }
    }
}