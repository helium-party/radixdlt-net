using HeliumParty.RadixDLT.Actions;
using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.EllipticCurve;
using HeliumParty.RadixDLT.EllipticCurve.Managers;
using HeliumParty.RadixDLT.Identity.Managers;
using HeliumParty.RadixDLT.Particles;
using HeliumParty.RadixDLT.Particles.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        private readonly IEUIDManager _euidManager;

        public SendMessageToParticleGroupsMapper(IECKeyManager keyManager, IEUIDManager euidManager)
            : this (keyManager,euidManager, sendmsg => 
            {
                return new List<ECPublicKey>()
                {
                    sendmsg.From.ECPublicKey,
                    sendmsg.To.ECPublicKey
                };
            })

        { }

        public SendMessageToParticleGroupsMapper(IECKeyManager keyManager, IEUIDManager euidManager, Func<SendMessageAction, IEnumerable<ECPublicKey>> encryptionScheme)
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
                var jsonstring = JsonConvert.SerializeObject(encrKeys.Select(k => k.Base64));
                var encryptorpayload = RadixConstants.StandardEncoding.GetBytes(jsonstring);

                MessageParticle encryptorParticle = new MessageParticleBuilder(_euidManager)
                    .PayLoad(encryptorpayload)
                    .AddMetaData("application","encryptor")
                    .AddMetaData("contentType", "application/json")
                    .From(action.From)
                    .To(action.To)
                    .Build();

                particles.Add(SpunParticle.Up(encryptorParticle));

                payload = _keyManager.Encrypt(sharedKeyPair.PublicKey, action.Data);
            }
            else
            {
                payload = action.Data;
            }

            var messageParticle = new MessageParticleBuilder(_euidManager)
                .PayLoad(payload)
                .AddMetaData("application", "message")
                .From(action.From)
                .To(action.To)
                .Build();

            particles.Add(SpunParticle.Up(messageParticle));

            return new List<ParticleGroup>() { new ParticleGroup(particles) };            
        }
    }
}
