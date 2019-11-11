using HeliumParty.RadixDLT.Actions;
using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.Identity.Managers;
using HeliumParty.RadixDLT.Particles;
using HeliumParty.RadixDLT.Particles.Types;
using HeliumParty.RadixDLT.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT.Mappers
{
    public class CreateTokenToParticleGroupsMapper : IStatelessActionToParticleGroupsMapper<CreateTokenAction>
    {
        private readonly IEUIDManager _euidManager;

        public CreateTokenToParticleGroupsMapper(IEUIDManager euidManager)
        {
            _euidManager = euidManager;
        }

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


            var tokens = new TransferableTokensParticle(
                token.RRI.Address,
                token.RRI,
                granularity,
                DateTimeOffset.Now.ToUnixTimeMilliseconds() / 60000L + 60000L,
                DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                amount,
                new Dictionary<TokenTransition, TokenPermission>(),
                _euidManager.GetEUID(token.Address));

            var rriParticle = new RRIParticle(token.RRI, _euidManager.GetEUID(token.RRI.Address));

            
            var tokenCreationGroup = new ParticleGroup(new List<SpunParticle>() 
            { 
                SpunParticle.Down(rriParticle),
                SpunParticle.Up(token),
                SpunParticle.Up(tokens)
            });

            return new List<ParticleGroup>() { tokenCreationGroup };
        }
    }
}
