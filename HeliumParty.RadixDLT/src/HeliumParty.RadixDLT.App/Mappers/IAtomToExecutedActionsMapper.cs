using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HeliumParty.RadixDLT.Mappers
{
    public interface IAtomToExcetedActionsMapper
    {

    }
    public interface IAtomToExecutedActionsMapper<T> : IAtomToExcetedActionsMapper
    {
        Type ActionClass(); 
        Task<T> Map(Atom a, IRadixIdentity Identity);
    }
}
