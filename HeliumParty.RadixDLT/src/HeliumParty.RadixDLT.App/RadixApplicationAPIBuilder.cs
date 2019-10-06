using HeliumParty.RadixDLT.Configuration;
using HeliumParty.RadixDLT.Mappers;
using HeliumParty.RadixDLT.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using HeliumParty.RadixDLT.State;
using System.Collections.Immutable;
using HeliumParty.RadixDLT.Actions;
using HeliumParty.RadixDLT.Particles;
using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.Universe;

namespace HeliumParty.RadixDLT
{
    public class RadixApplicationAPIBuilder
    {
        private IRadixIdentity _identity;
        private RadixUniverse _universe;//needs networklayer
        private IFeeMapper _feeMapper;
        private List<IParticleReducer> _reducers = new List<IParticleReducer>();
        private List<IAtomToExcetedActionsMapper> _atomMappers = new List<IAtomToExcetedActionsMapper>();
        private List<IAtomErrorToExceptionReasonMapper> _atomErrorMappers = new List<IAtomErrorToExceptionReasonMapper>();
        private ImmutableDictionary<Type, Func<IAction, HashSet<ShardedParticleStateId>>>.Builder _requiredStateMappers;
        private ImmutableDictionary<Type, Func<IAction, IEnumerable<Particle>, List<ParticleGroup>>>.Builder _actionMappers;

        public RadixApplicationAPIBuilder()
        {            
        }

        public RadixApplicationAPIBuilder Bootstrap(IBootstrapConfig config)
        {
            _universe = RadixUniverse.Create(config);
            return this;
        }

        public RadixApplicationAPIBuilder DefaultFeeMapper()
        {
            _feeMapper = new PowFeeMapper();
            return this;
        }

        public RadixApplicationAPIBuilder Identity(IRadixIdentity identity)
        {
            _identity = identity;
            return this;
        }

        public RadixApplicationAPIBuilder Universe(RadixUniverse universe)
        {
            _universe = universe;
            return this;
        }

        public RadixApplicationAPIBuilder FeeMapper(IFeeMapper feeMapper)
        {
            _feeMapper = feeMapper;
            return this;
        }

        public static RadixApplicationAPIBuilder Builder()
        {
            return new RadixApplicationAPIBuilder();            
        }

        public RadixApplicationAPIBuilder AddAtomMapper<T>(IAtomToExecutedActionsMapper<T> atomMapper)
        {
            _atomMappers.Add(atomMapper);
            return this;
        }

        public RadixApplicationAPIBuilder AddReducer<T>(IParticleReducer<T> reducer)
            where T : IApplicationState
        {            
            _reducers.Add(reducer);
            return this;
        }

        public RadixApplicationAPIBuilder AddStatelessParticlesMapper<T>(
            IStatelessActionToParticleGroupsMapper<T> mapper)
            where T : IAction
        {
            _requiredStateMappers.Add(typeof(T), a => new HashSet<ShardedParticleStateId>());
            _actionMappers.Add(typeof(T), (a, p) => mapper.MapToParticleGroup((T)a));
            return this;
        }

        public RadixApplicationAPIBuilder AddStateFullParticlesMapper<T>(
            IStatefulActionToParticleGroupsMapper<T> mapper)
            where T : IAction
        {
            _requiredStateMappers.Add(typeof(T), a => mapper.RequiredState((T)a));
            _actionMappers.Add(typeof(T), (a, p) => mapper.MapToParticleGroups((T)a, p));
            return this;
        }

        public RadixApplicationAPIBuilder AddAtomErrorMapper(IAtomErrorToExceptionReasonMapper mapper)
        {
            _atomErrorMappers.Add(mapper);
            return this;
        }

        public RadixApplicationAPI Build()
        {
            return new RadixApplicationAPI(
                _identity ?? throw new ArgumentNullException(nameof(_identity)),
                _universe ?? throw new ArgumentNullException(nameof(_universe)),
                _feeMapper ?? throw new ArgumentNullException(nameof(_feeMapper)),
                _requiredStateMappers.ToImmutableDictionary(),
                _actionMappers.ToImmutableDictionary(),
                _reducers,
                _atomMappers,
                _atomErrorMappers
                );
        }
    }    
}
