using HeliumParty.RadixDLT.Jsonrpc;
using HeliumParty.RadixDLT.Universe;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace HeliumParty.RadixDLT.Epics
{
    /// <summary>
    /// Epic which executes Json Rpc methods over the websocket as Json Rpc requests come in. 
    /// The responses are emitted.
    /// </summary>
    /// <typeparam name="T">Which <see cref="Actions.IJsonRpcMethodAction"/> are processed</typeparam>
    public class RadixJsonRpcMethodEpic<T, JAction> : IRadixNetworkEpic where T: Actions.IJsonRpcMethodAction
    {
        private readonly WebSockets _WebSockets;
        private readonly Func<RadixJsonRpcClient, T, IObservable<Actions.IJsonRpcResultAction<JAction>>> _MethodCall;

        public RadixJsonRpcMethodEpic(
            WebSockets webSockets, 
            Func<RadixJsonRpcClient, T, IObservable<Actions.IJsonRpcResultAction<JAction>>> methodCall)
        {
            _WebSockets = webSockets ?? throw new ArgumentNullException(nameof(webSockets));
            _MethodCall = methodCall ?? throw new ArgumentNullException(nameof(methodCall));
        }

        /// <summary>
        /// Returns a <see cref="Web.WebSocketClient"/> after it is connected to the specific node
        /// </summary>
        /// <param name="node">The node to connect to</param>
        /// <returns>An observable that emits a connected <see cref="Web.WebSocketClient"/></returns>
        private IObservable<Web.WebSocketClient> WaitForConnection(RadixNode node)
        {
            var obs = Utils.WaitForConnection(_WebSockets, node, out var ws);
            
            return obs
                .Select(s => ws)
                .FirstAsync();
        }
        
        public IObservable<IRadixNodeAction> Epic(IObservable<IRadixNodeAction> actions, IObservable<RadixNetworkState> networkState)
        {
            return actions
                .OfType<T>()
                .SelectMany(a => WaitForConnection(a.Node)
                    .Select(wsc => new Jsonrpc.RadixJsonRpcClient(wsc))
                    .SelectMany(c => _MethodCall(c, a)));
        }

        public static IRadixNetworkEpic CreateGetLivePeersEpic(WebSockets webSockets)
        {
            return new RadixJsonRpcMethodEpic<Actions.GetLivePeersRequestAction, List<NodeRunnerData>>(
                webSockets,
                (client, action) => client.GetLivePeers().Select(d => new Actions.GetLivePeersResultAction(action.Node, d)));
        }

        public static IRadixNetworkEpic CreateGetNodeDataEpic(WebSockets webSockets)
        {
            return new RadixJsonRpcMethodEpic<Actions.GetNodeDataRequestAction, NodeRunnerData>(
                webSockets,
                (client, action) => client.GetNodeData().Select(d => new Actions.GetNodeDataResultAction(action.Node, d)));
        }

        public static IRadixNetworkEpic CreateGetUniverseEpic(WebSockets webSockets)
        {
            return new RadixJsonRpcMethodEpic<Actions.GetUniverseRequestAction, RadixUniverseConfig>(
                webSockets,
                (client, action) => client.GetUniverseConfig().Select(u => new Actions.GetUniverseResponseAction(action.Node, u)));
        }
    }
}
