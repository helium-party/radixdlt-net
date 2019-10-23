using HeliumParty.RadixDLT.Actions;
using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.EllipticCurve;
using HeliumParty.RadixDLT.EllipticCurve.Managers;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT.Mappers
{
    public class SendMessageToParticleGroupsMapper : IStatelessActionToParticleGroupsMapper<SendMessageAction>
    {
        private readonly IECKeyManager _keyManager;
        private readonly Func<SendMessageAction, IEnumerable<ECPublicKey>> _encryptionScheme;
        public List<ParticleGroup> MapToParticleGroup(SendMessageAction action)
        {
            throw new NotImplementedException();
        }
    }
}
