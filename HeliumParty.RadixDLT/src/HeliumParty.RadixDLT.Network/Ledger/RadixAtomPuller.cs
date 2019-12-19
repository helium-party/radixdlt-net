using System;
using HeliumParty.RadixDLT.Identity;

namespace HeliumParty.RadixDLT.Ledger
{
    /// <summary>
    /// Module responsible for fetches and merges of new atoms into the atom store
    /// </summary>
    public class RadixAtomPuller : IAtomPuller
    {




        public IObservable<object> Pull(RadixAddress address)
        {
            throw new NotImplementedException();
        }
    }
}
