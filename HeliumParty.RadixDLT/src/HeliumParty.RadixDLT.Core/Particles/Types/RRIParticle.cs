using System.Collections.Generic;
using Dahomey.Cbor.Attributes;
using HeliumParty.RadixDLT.Identity;
using Newtonsoft.Json;

namespace HeliumParty.RadixDLT.Particles.Types
{
    [CborDiscriminator("radix.particles.rri", Policy = CborDiscriminatorPolicy.Always)]
    [JsonObject(ItemTypeNameHandling = TypeNameHandling.None)]
    public class RRIParticle : Particle, IAccountable
    {
        public RRI RRI { get; protected set; }
        public long Nonce { get; protected set; }

        public HashSet<RadixAddress> Addresses => new HashSet<RadixAddress>() { RRI.Address };

        protected RRIParticle() : base()
        {

        }

        //destination should originate from the address the rri represents
        public RRIParticle(RRI rri, EUID destination) : base(destination)
        {
            RRI = rri;
            Nonce = 0;
        }
    }
}