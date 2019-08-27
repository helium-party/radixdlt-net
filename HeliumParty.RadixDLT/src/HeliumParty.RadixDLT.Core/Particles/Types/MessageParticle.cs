using System.Collections.Generic;
using Dahomey.Cbor.Attributes;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Identity.Managers;
using Newtonsoft.Json;

namespace HeliumParty.RadixDLT.Particles.Types
{
    [JsonObject(MemberSerialization.OptIn)]
    public class MessageParticle : Particle, IAccountable
    {
        [CborProperty("from"), JsonProperty(PropertyName = "from")]
        public RadixAddress From { get; }

        [CborProperty("to"), JsonProperty(PropertyName = "to")]
        public RadixAddress To { get; }
        
        //aka content-type
        [CborProperty("metaData"), JsonProperty(PropertyName = "metaData")]
        public IDictionary<string, string> MetaData { get; }

        //aka data, message,...
        [CborProperty("bytes"), JsonProperty(PropertyName = "bytes")]
        public byte[] Bytes { get; }

        [CborProperty("nonce"), JsonProperty(PropertyName = "nonce")]
        public long Nonce { get; }

        public HashSet<RadixAddress> Addresses => new HashSet<RadixAddress> { From, To };

        public MessageParticle(RadixAddress from, RadixAddress to, IDictionary<string, string> metaData, byte[] bytes)
            : this(from, to, metaData, bytes, RandomGenerator.GetRandomLong(), ConvertToEUID(from, to)) { }

        [CborConstructor, JsonConstructor]
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