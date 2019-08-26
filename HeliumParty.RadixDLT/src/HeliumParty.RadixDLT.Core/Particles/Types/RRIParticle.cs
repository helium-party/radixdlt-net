using System.Collections.Generic;
using Dahomey.Cbor.Attributes;
using HeliumParty.RadixDLT.Identity;
using Newtonsoft.Json;

namespace HeliumParty.RadixDLT.Particles.Types
{
    public class RRIParticle : Particle, IAccountable
    {
        [CborProperty("rri"), JsonProperty(PropertyName = "rri")]
        public RRI RRI { get; protected set; }

        [CborProperty("nonce"), JsonProperty(PropertyName = "nonce")]
        public long Nonce { get; }

        public HashSet<RadixAddress> Addresses => new HashSet<RadixAddress>() { RRI.Address };
        
        public RRIParticle(RRI rri) : base(rri.Address.EUID)
        {
            RRI = rri;
            Nonce = 0;
        }
    }
}