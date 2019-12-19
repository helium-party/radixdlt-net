using HeliumParty.RadixDLT.Identity;
using System;

namespace HeliumParty.RadixDLT.Ledger
{
    /// <summary>
    /// Temporary interface for retrieving atoms at an address from the network
    /// TODO: Might get changed / refactored in java
    /// </summary>
    public interface IAtomPuller
    {
        /// <summary>
        /// Fetches atoms and stores them in an <see cref="IAtomStore"/>
        /// </summary>
        /// <param name="address">The address to pull atoms from</param>
        /// <returns>An <see cref="IObservable{T}"/> corresponding to the atoms fetched and pushed to the store</returns>
        IObservable<object> Pull(RadixAddress address);
    }
}
