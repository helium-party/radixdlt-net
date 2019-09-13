using System.Collections.Generic;
using Dahomey.Cbor.Attributes;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Identity.Managers;

namespace HeliumParty.RadixDLT.Particles.Types
{
    [CborDiscriminator("radix.particles.message",Policy = CborDiscriminatorPolicy.Always)]
    public class MessageParticle : Particle, IAccountable
    {
        public RadixAddress From { get; protected set; }
        public RadixAddress To { get; protected set; }

        //aka content-type
        public IDictionary<string, string> MetaData { get; protected set; }
        //aka data, message,...
        public byte[] Bytes { get; protected set; }
        public long Nonce { get; protected set; }

        public HashSet<RadixAddress> Addresses => new HashSet<RadixAddress> { From, To };

        protected MessageParticle() : base()
        {

        }

        public MessageParticle(RadixAddress from, RadixAddress to, IDictionary<string, string> metaData, byte[] bytes)
            : this(from, to, metaData, bytes, RandomGenerator.GetRandomLong(), ConvertToEUID(from, to)) { }

        public MessageParticle(RadixAddress from, RadixAddress to, IDictionary<string, string> metaData, byte[] bytes, long nonce, HashSet<EUID> destinations)
            : base(destinations)
        {
            From = from;
            To = to;
            MetaData = metaData;
            Bytes = bytes;
            Nonce = nonce;
        }

        private static HashSet<EUID> ConvertToEUID(RadixAddress from, RadixAddress to)
        {            
            return new HashSet<EUID>() { from.EUID, to.EUID };
        }
    }
}