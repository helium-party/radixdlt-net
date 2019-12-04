using HeliumParty.RadixDLT.Jsonrpc;
using System;
using System.Reactive.Linq;

namespace HeliumParty.RadixDLT.Epics
{
    /// <summary>
    /// Epic which executes Json Rpc methods over the websocket as Json Rpc requests come in. 
    /// The responses are emitted
    /// </summary>
    /// <typeparam name="T">Which <see cref="Actions.IJsonRpcMethodAction"/> are processed</typeparam>
    public class RadixJsonRpcMethodEpic<T> : IRadixNetworkEpic where T: Actions.IJsonRpcMethodAction
    {
        private readonly WebSockets _WebSockets;
        private readonly Func<RadixJsonRpcClient, T, IObservable<Actions.IJsonRpcResultAction<T>>> _MethodCall; // TODO: huhhh...
        private readonly T _JsonMethod;
                
        public RadixJsonRpcMethodEpic(
            WebSockets webSockets, 
            Func<RadixJsonRpcClient, T, IObservable<Actions.IJsonRpcResultAction<T>>> methodCall,
            T jsonMethodAction)
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
            Web.WebSocketClient ws = _WebSockets.GetOrCreate(node);
            return ws.State.Do(s =>
            {
                if (s.Equals(Web.WebSocketStatus.Disconnected))
                    ws.Connect();
            })
            .Where(s => s.Equals(Web.WebSocketStatus.Connected))
            .FirstAsync()
            .Select(s => ws);
        }
        
        public IObservable<IRadixNodeAction> Epic(IObservable<IRadixNodeAction> actions, IObservable<RadixNetworkState> networkState)
        {
            return actions
                .OfType<T>()
                .SelectMany(a => WaitForConnection(a.Node)
                    .Select(wsc => new Jsonrpc.RadixJsonRpcClient(wsc))
                   // FINISH THIS
        }
    }
}
