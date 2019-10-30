using HeliumParty.RadixDLT.Actions;
using HeliumParty.RadixDLT.Atoms;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT.Mappers
{
    public class CreateTokenToParticleGroupsMapper : IStatelessActionToParticleGroupsMapper<CreateTokenAction>
    {
        public List<ParticleGroup> MapToParticleGroups(CreateTokenAction action)
        {
            if (action.TokenSupplyType == TokenSupplyType.Fixed)
            {
                return createFixedSupplyToken(tokenCreation);
            }
            else if (action.TokenSupplyType == TokenSupplyType.Mutable)
            {
                return createVariableSupplyToken(action);
            }
            else
            {
                throw new ArgumentException("Unknown supply type: " + action.TokenSupplyType);
            }
        }
    }
}
