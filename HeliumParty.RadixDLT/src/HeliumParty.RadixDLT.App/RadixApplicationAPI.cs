using HeliumParty.RadixDLT.Actions;
using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.EllipticCurve;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Mappers;
using HeliumParty.RadixDLT.Particles;
using HeliumParty.RadixDLT.State;
using HeliumParty.RadixDLT.Universe;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace HeliumParty.RadixDLT
{
    public class RadixApplicationAPI
    {        
        private RadixUniverse _universe;//needs networklayer
        private IFeeMapper _feeMapper;
        private List<IParticleReducer> _reducers = new List<IParticleReducer>();
        private ImmutableDictionary<Type, Func<IAction, HashSet<ShardedParticleStateId>>> _requiredStateMappers;
        private ImmutableDictionary<Type, Func<IAction, IEnumerable<Particle>, List<ParticleGroup>>> _actionMappers;
        private List<IAtomToExcetedActionsMapper> _atomMappers = new List<IAtomToExcetedActionsMapper>();
        private List<IAtomErrorToExceptionReasonMapper> _atomErrorMappers = new List<IAtomErrorToExceptionReasonMapper>();

        public IRadixIdentity Identity { get; }
        public ECPublicKey PublicKey => Identity.PublicKey;
        public RadixAddress Address => new RadixAddress(_universe.Magic, PublicKey);


        internal RadixApplicationAPI(
            IRadixIdentity identity, 
            RadixUniverse universe, 
            IFeeMapper feeMapper,                          
            ImmutableDictionary<Type, Func<IAction, HashSet<ShardedParticleStateId>>> requiredStateMappers, 
            ImmutableDictionary<Type, Func<IAction, IEnumerable<Particle>, List<ParticleGroup>>> actionMappers,
            List<IParticleReducer> reducers,
            List<IAtomToExcetedActionsMapper> atomMappers,
            List<IAtomErrorToExceptionReasonMapper> atomErrorMappers)
        {
            Identity = identity;
            _universe = universe;
            _feeMapper = feeMapper;
            _reducers = reducers;
            _atomMappers = atomMappers;
            _atomErrorMappers = atomErrorMappers;
            _requiredStateMappers = requiredStateMappers;
            _actionMappers = actionMappers;
        }



    }

}
