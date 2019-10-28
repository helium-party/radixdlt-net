using System;
using System.Linq;
using System.Reactive.Linq;

namespace HeliumParty.RadixDLT.Epics
{
    /// <summary>
    /// Epic which manages simple bootstrapping and discovers nodes one degree out from the initial seed
    /// </summary>
    public class DiscoverNodesEpic : IRadixNetworkEpic
    {
        private readonly IObservable<RadixNode> _Seeds;
        private readonly Universe.RadixUniverseConfig _Config;

        public DiscoverNodesEpic(IObservable<RadixNode> seeds, Universe.RadixUniverseConfig config)
        {
            _Seeds = seeds ?? throw new ArgumentNullException(nameof(seeds));
            _Config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public IObservable<IRadixNodeAction> Epic(IObservable<IRadixNodeAction> actions, IObservable<RadixNetworkState> networkState)
        {
            IObservable<IRadixNodeAction> getSeedUniverse =
                actions
                .OfType<Actions.DiscoverMoreNodesAction>()
                .FirstAsync()   // we aren't actually async, however the other methods are marked as deprecated
                .SelectMany(i => _Seeds)
                .Select(node => Actions.GetUniverseRequestAction.From(node))
                .Catch<IRadixNodeAction, System.Exception>(exc =>                   //  TODO: Is this whole logic really necessary for new 'DiscoverMoreNodesErrorAction'? 
                                                                                    //        should this be moved into the action class itself?
                {
                    return Observable.Create(
                        (IObserver<Actions.DiscoverMoreNodesErrorAction> observer) =>
                        {
                            observer.OnNext(new Actions.DiscoverMoreNodesErrorAction(exc));
                            return System.Reactive.Disposables.Disposable.Empty;
                        });
                });

            IObservable<IRadixNodeAction> seedUniverseMismatch =
                actions
                .OfType<Actions.GetUniverseResponseAction>()
                .Where(action => !action.GetResult().Equals(_Config))
                .Select(action => new Actions.NodeUniverseMismatchAction(action.Node, _Config, action.GetResult());

            IObservable<RadixNode> connectedSeeds =
                actions
                .OfType<Actions.GetUniverseResponseAction>()
                .Where(action => action.GetResult().Equals(_Config))
                .Select(action => action.Node)
                .Publish()
                .AutoConnect(3);    // TODO: Make a constant for this?

            IObservable<IRadixNodeAction> addSeeds = connectedSeeds.Select(Actions.AddNodeAction.From);
            IObservable<IRadixNodeAction> addSeedData = connectedSeeds.Select(Actions.GetNodeDataRequestAction.From);
            IObservable<IRadixNodeAction> addSeedSiblings = connectedSeeds.Select(Actions.GetLivePeersRequestAction.From);

            IObservable<IRadixNodeAction> addNodes =
                actions
                .OfType<Actions.GetLivePeersResultAction>()
                .SelectMany(action =>
                    Observable.CombineLatest(
                        Observable.Return(action.GetResult()),
                        Observable.Concat(networkState.FirstAsync(), Observable.Never<RadixNetworkState>()),
                        (data, state) =>
                        {
                            return data.Select(d =>
                            {
                                RadixNode node = new RadixNode(d.GetIP(), action.Node.Port, action.Node.IsSSL);
                                return state.NodeStateCollection.ContainsKey(node) ? null : Actions.AddNodeAction.From(node);
                            })
                            .Where(addNode => addNode != null);
                        }))
                .SelectMany(i => i);   // transform the IEnumerable of actions back into our usual 'IObservable<IRadixNodeAction>' - observable

            return Observable.Merge(
                addSeeds,
                addSeedData,
                addSeedSiblings,
                addNodes,
                getSeedUniverse,
                seedUniverseMismatch);
        }
    }
}
