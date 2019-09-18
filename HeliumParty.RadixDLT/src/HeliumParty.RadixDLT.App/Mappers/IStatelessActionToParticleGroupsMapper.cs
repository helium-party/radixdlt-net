using HeliumParty.RadixDLT.Actions;
using HeliumParty.RadixDLT.Atoms;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT.Mappers
{
    /// <summary>
    ///     maps a high level application action to a lower level spun particles used to construct an atom
    /// </summary>
    public interface IStatelessActionToParticleGroupsMapper<T>
        where T : IAction
    {
        List<ParticleGroup> MapToParticleGroup(T action);
    }
}
