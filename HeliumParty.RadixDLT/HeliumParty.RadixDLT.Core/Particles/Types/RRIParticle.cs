using System.Collections.Generic;
using HeliumParty.RadixDLT.Identity;

namespace HeliumParty.RadixDLT.Particles.Types
{
    public class RRIParticle : Particle, IAccountable
    {
        public RRI RRI { get; protected set; }
        public long Nonce { get; }

        public HashSet<RadixAddress> Addresses => new HashSet<RadixAddress>() { RRI.Address };

        public RRIParticle(RRI rri) : base(rri.Address.EUID)
        {
            RRI = rri;
            Nonce = 0;
        }
    }
}