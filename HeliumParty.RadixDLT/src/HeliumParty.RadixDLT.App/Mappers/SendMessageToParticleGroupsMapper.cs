using HeliumParty.RadixDLT.Actions;
using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.EllipticCurve;
using HeliumParty.RadixDLT.EllipticCurve.Managers;
using HeliumParty.RadixDLT.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeliumParty.RadixDLT.Mappers
{
    public class SendMessageToParticleGroupsMapper : IStatelessActionToParticleGroupsMapper<SendMessageAction>
    {
        private readonly Func<SendMessageAction, IEnumerable<ECPublicKey>> _encryptionScheme;

        private readonly IECKeyManager _keyManager;

        public SendMessageToParticleGroupsMapper(IECKeyManager keyManager)
            : this (keyManager, sendmsg => 
            {
                return new List<ECPublicKey>()
                {
                    sendmsg.From.ECPublicKey,
                    sendmsg.To.ECPublicKey
                };
            })

        { }

        public SendMessageToParticleGroupsMapper(IECKeyManager keyManager, Func<SendMessageAction, IEnumerable<ECPublicKey>> encryptionScheme)
        {
            _encryptionScheme = encryptionScheme;
            _keyManager = keyManager;
        }

        public List<ParticleGroup> MapToParticleGroups(SendMessageAction action)
        {
            var particles = new List<SpunParticle>();

            byte[] payload;
            if (action.Encrypt)
            {
                var keys = _encryptionScheme.Invoke(action).ToList();
                var sharedKeyPair = _keyManager.GetRandomKeyPair();
                var encrKeys = new List<EncryptedPrivateKey>();

                foreach(var k in keys)
                    encrKeys.Add(_keyManager.CreateSharedKey(k, sharedKeyPair.PrivateKey));
                
                // to do : 
                //  1)parse all encryptedprivatekeys to json string. 
                //  2)parse json to bytes with UTF8 encoding. 
                //  3)build message particle
                //  4)add particle to spunparticle as up
                //  5)encrypt data by sharedpublickey(from action) load it in payload
                

                

            }
            else
            {
                payload = action.Data;
            }

            //var messageParticle = new MessageParticleBuilder()

            throw new NotImplementedException();
        }
    }
}
