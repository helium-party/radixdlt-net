using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace HeliumParty.RadixDLT.Epics
{
    /// <summary>
    /// Epic which finds a connected sharded node when a 'FindANode' request is received. 
    /// If there are none found, then the epic attempts to start connections
    /// </summary>
    public class FindANodeEpic : IRadixNetworkEpic
    {
        private const int MaxSimultaneousConnectionRequests = 2;
        private static readonly TimeSpan NextConnectionThrottleTimeout = TimeSpan.FromSeconds(1);

        private static readonly Log.ILogger _Logger = new Log.OutputLogger();
        private readonly Selectors.IRadixPeerSelector _Selector;

        public FindANodeEpic(Selectors.IRadixPeerSelector selector) 
        {
            _Selector = selector ?? throw new ArgumentNullException(nameof(selector));
        }

        private List<IRadixNodeAction> NextConnectionRequest(HashSet<long> shards, RadixNetworkState state)
        {
            // Dictionary that lists all nodes that have the same status
            var statusDic = new Dictionary<Web.WebSocketStatus, List<RadixNode>>();
            foreach (Web.WebSocketStatus wss in Enum.GetValues(typeof(Web.WebSocketStatus)))
                statusDic.Add(
                    wss,
                    state.NodeStateCollection
                        .Where(pair => { return pair.Value.Status.Equals(wss); })
                        .Select(pair => pair.Key).ToList());

            var connectingNodeCount = statusDic[Web.WebSocketStatus.Connecting].Count;
            
            if (connectingNodeCount < MaxSimultaneousConnectionRequests)
            {
                var disconnectedPeers = statusDic[Web.WebSocketStatus.Disconnected];
                if (disconnectedPeers.Count == 0)
                    return new List<IRadixNodeAction>{ new Actions.DiscoverMoreNodesAction() };

                else
                {
                    List<RadixNode> correctShardNodes = new List<RadixNode>(
                        disconnectedPeers
                        .Where(node => state.NodeStateCollection[node].Shards.Intersects(shards))
                        );

                    if (correctShardNodes.Count == 0)
                    {
                        List<RadixNode> unknownShardNodes = new List<RadixNode>(
                            disconnectedPeers
                            .Where(node =>
                                state.NodeStateCollection[node].Shards == null
                                ));

                        if (unknownShardNodes.Count == 0)
                            return new List<IRadixNodeAction> { new Actions.DiscoverMoreNodesAction() };

                        else
                        {
                            return unknownShardNodes
                                .SelectMany(node => new IRadixNodeAction[] {
                                    new Actions.GetNodeDataRequestAction(node),
                                    new Actions.GetUniverseRequestAction(node)})
                                .ToList();
                        }
                    }
                    else
                        return new List<IRadixNodeAction>() {
                            new Actions.ConnectionWebSocketAction(_Selector.Apply(correctShardNodes))};
                }
            }

            return new List<IRadixNodeAction>();
        }
            
        /// <summary>
        /// Fetches all connected nodes that intersect the specified shard
        /// </summary>
        /// <param name="shards">The node must intersect with this shard</param>
        /// <param name="state">Information about all connected nodes</param>
        /// <returns>All nodes we are connected to that intersec specified shard</returns>
        private static List<RadixNode> GetConnectedNodes(HashSet<long> shards, RadixNetworkState state)
        {
            return state.NodeStateCollection
                .Where(entry => entry.Value.Status == Web.WebSocketStatus.Connected)
                .Where(entry => entry.Value.Shards.Intersects(shards))
                .Select(entry => entry.Key)
                .ToList();
        }

        public IObservable<IRadixNodeAction> Epic(IObservable<IRadixNodeAction> actions, IObservable<RadixNetworkState> networkState)
        {
            return actions.OfType<Actions.IFindANodeRequestAction>()
                .SelectMany(action =>
                {
                    IObservable<List<RadixNode>> connectedNodes =
                        networkState
                            .Select(state => GetConnectedNodes(action.Shards, state))
                            .Replay(1)
                            .AutoConnect(2);    // TODO: Make constant?

                    // Stream to find node
                    IObservable<IRadixNodeAction> selectedNode =
                        connectedNodes
                            .Where(nodes => nodes.Count > 0)
                            .FirstAsync()
                            .Select(_Selector.Apply)
                            .Select(node => new Actions.FindANodeResultAction(node, action))
                            .Replay();

                    // Stream of new actions to find a new node
                    IObservable<IRadixNodeAction> findConnectionActions =
                        connectedNodes
                            .Where(nodes => nodes.Count == 0)
                            .Select(_ => default(IRadixNodeAction))     // Cast back to our usual IRadixNodeAction 
                            .FirstAsync()
                            .IgnoreElements()
                            .Concat(
                                Observable
                                   .Interval(NextConnectionThrottleTimeout)
                                   .WithLatestFrom(networkState, (i, s) => s)
                                   .SelectMany(state => NextConnectionRequest(action.Shards, state))
                                    )
                            .TakeUntil(selectedNode)
                            .Replay(1)
                            .AutoConnect(2);

                    // Cleanup and close connections which never worked out
                    IObservable<IRadixNodeAction> cleanupConnections =
                        findConnectionActions
                            .OfType<Actions.ConnectionWebSocketAction>()
                            .SelectMany(c =>
                            {
                                var node = c.Node;
                                return selectedNode
                                    .Select(sn => sn.Node)
                                    .Where(sn => !node.Equals(sn))
                                    .Select(sn => new Actions.CloseWebSocketAction(sn));
                            });

                    return findConnectionActions
                    .Concat(selectedNode)
                    .Merge(cleanupConnections);
                });
        }
    }
}
