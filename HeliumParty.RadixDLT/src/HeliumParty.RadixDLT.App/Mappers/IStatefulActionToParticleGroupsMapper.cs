using HeliumParty.RadixDLT.Actions;
using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.Particles;
using HeliumParty.RadixDLT.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT.Mappers
{
    /// <summary>
    ///     Maps a high level application action to lower level spun particles used
    ///     to construct an atom given a context requirement which this interface describes
    ///     via requiredState().
    /// </summary>
    public interface IStatefulActionToParticleGroupsMapper<T>
        where T : IAction
    {
        /// <summary>
        ///     Retrieves the necessary application state to be used in creating new particles
        ///     given a high level action. 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        HashSet<ShardedParticleStateId> RequiredState(T action);

        List<ParticleGroup> MapToParticleGroups(T action, IEnumerable<Particle> store);
    }
}
