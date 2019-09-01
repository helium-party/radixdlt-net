using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HeliumParty.RadixDLT.Mapper
{
    public interface IAtomToExecutedActionsMapper<T>
    {
        //class<T> ActionClass(); need a work around
        Task<T> Map(Atom a, IRadixIdentity Identity);
    }
}
