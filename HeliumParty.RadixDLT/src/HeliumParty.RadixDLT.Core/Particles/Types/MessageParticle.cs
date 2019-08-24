using System.Collections.Generic;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Identity.Managers;

namespace HeliumParty.RadixDLT.Particles.Types
{
    public class MessageParticle : Particle, IAccountable
    {
        public RadixAddress From { get; }
        public RadixAddress To { get; }

        //aka content-type
        public IDictionary<string, string> MetaData { get; }
        //aka data, message,...
        public byte[] Bytes { get; }
        public long Nonce { get; }

        public HashSet<RadixAddress> Addresses => new HashSet<RadixAddress> { From, To };

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
            var manager = new EUIDManager();
            return new HashSet<EUID>() { from.EUID, to.EUID };
        }
    }
}