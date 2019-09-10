using System.Collections.Generic;
using Dahomey.Cbor.Attributes;
using HeliumParty.RadixDLT.Identity;

namespace HeliumParty.RadixDLT.Particles.Types
{
    [CborDiscriminator("radix.particles.rri")]
    public class RRIParticle : Particle, IAccountable
    {
        public RRI RRI { get; protected set; }
        public long Nonce { get; protected set; }

        public HashSet<RadixAddress> Addresses => new HashSet<RadixAddress>() { RRI.Address };
        
        protected RRIParticle()
            : base()
        {

        }

        public RRIParticle(RRI rri) : base(rri.Address.EUID)
        {
            RRI = rri;
            Nonce = 0;
        }
    }
}