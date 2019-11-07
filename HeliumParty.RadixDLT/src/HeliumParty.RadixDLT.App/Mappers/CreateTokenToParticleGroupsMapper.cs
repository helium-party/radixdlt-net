using HeliumParty.RadixDLT.Actions;
using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.Particles.Types;
using HeliumParty.RadixDLT.Primitives;
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
                return createFixedSupplyToken(action);
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

        private List<ParticleGroup> createVariableSupplyToken(CreateTokenAction action)
        {
            throw new NotImplementedException();
        }

        private List<ParticleGroup> createFixedSupplyToken(CreateTokenAction action)
        {
            var amount = TokenUnitConversions.UnitsToSubUnits(action.InitialSupply);
            var granularity = TokenUnitConversions.UnitsToSubUnits(action.Granularity);

            var token = new FixedSupplyTokenDefinitionParticle(
                action.RRI.Address,
                action.RRI.Name,
                action.Name,
                action.Description,
                amount,
                granularity,
                action.IconUrl);

            
            throw new NotImplementedException();
        }
    }
}
