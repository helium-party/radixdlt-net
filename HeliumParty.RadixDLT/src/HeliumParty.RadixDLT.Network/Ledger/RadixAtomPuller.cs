using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using HeliumParty.RadixDLT.Identity;

namespace HeliumParty.RadixDLT.Ledger
{
    /// <summary>
    /// Module responsible for fetches and merges of new atoms into the atom store
    /// </summary>
    public class RadixAtomPuller : IAtomPuller
    {
        /// <summary>
        /// Atoms retrieved from the network
        /// </summary>
        private readonly ConcurrentDictionary<RadixAddress, IObservable<object>> _Cache = 
            new ConcurrentDictionary<RadixAddress, IObservable<object>>();

        /// <summary>
        /// The mechanism by which to fetch atoms
        /// </summary>
        private readonly RadixNetworkController _Controller;

        public RadixAtomPuller(RadixNetworkController controller)
        {
            _Controller = controller ?? throw new ArgumentNullException(nameof(controller));
        }

        /// <summary>
        /// Fetches atoms and pushes them into the atom store. 
        /// Multiple pulls on the same address will return a disposable to 
        /// the same observable. As long as there is one subscriber to an 
        /// address this will continue fetching and storing atoms. 
        /// (By usage of Observable's Publish-RefCount pattern)
        /// </summary>
        /// <param name="address">Shard address to get atoms from</param>
        /// <returns>Disposable Observable to stop fetching</returns>
        public IObservable<object> Pull(RadixAddress address)
        {
            return _Cache.GetOrAdd(
                address,
                addr =>
                {
                    return Observable.Create<object>(emitter =>
                    {
                        var fetchAction = new Actions.FetchAtomsRequestAction(addr);
                        _Controller.Dispatch(fetchAction);

                        return Disposable.Create(() => 
                            _Controller.Dispatch(new Actions.FetchAtomsCancelAction(fetchAction.UUID, addr)));
                    })
                    .Publish()
                    .RefCount();
                });
        }
    }
}
