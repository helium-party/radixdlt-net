using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Identity.Managers;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT.Particles.Types
{
    public class MessageParticleBuilder
    {
        private long _nonce = RandomGenerator.GetRandomLong();

        private RadixAddress _from;
        private RadixAddress _to;
        private IDictionary<string, string> _metaData = new Dictionary<string,string>();
        private byte[] _data;

        private readonly IEUIDManager _idManager;

        public MessageParticleBuilder PayLoad(byte[] data)
        {
            _data = data;
            return this;
        }

        public MessageParticleBuilder(IEUIDManager idManager)
        {
            _idManager = idManager;
        }

        public MessageParticleBuilder From(RadixAddress from)
        {
            _from = from;
            return this;
        }

        public MessageParticleBuilder To(RadixAddress to)
        {
            _to = to;
            return this;
        }

        public MessageParticleBuilder AddMetaData(string key, string value)
        {
            _metaData.Add(key, value);
            return this;
        }

        public MessageParticleBuilder Nonce (long nonce)
        {
            _nonce = nonce;
            return this;
        }

        public MessageParticle Build()
        {
            return new MessageParticle(_from, _to, _metaData, _data,_nonce,
                new HashSet<EUID>
                {
                    _idManager.GetEUID(_from),
                    _idManager.GetEUID(_to)
                });
        }
    }
}
