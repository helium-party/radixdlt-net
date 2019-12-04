using System;
using System.Reactive.Linq;

namespace HeliumParty.RadixDLT.Epics
{
    /// <summary>
    /// Epic which attempts to open a websocket when a Json Rpc method onto a given node is dispatched
    /// </summary>
    public class RadixJsonRpcAutoConnectEpic : IRadixNetworkEpic
    {
        private readonly WebSockets _WebSockets;

        public RadixJsonRpcAutoConnectEpic(WebSockets webSockets)
        {
            _WebSockets = webSockets ?? throw new ArgumentNullException(nameof(webSockets));
        }

        public IObservable<IRadixNodeAction> Epic(IObservable<IRadixNodeAction> actions, IObservable<RadixNetworkState> networkState)
        {
            return
                actions
                    .Where(a => Generics.InheritsOrIsInstance(a.GetType(), typeof(Actions.IJsonRpcMethodAction)))
                    .SelectMany(a =>
                    {
                        var ws = _WebSockets.GetOrCreate(a.Node);
                        return ws.State
                            .Do(s =>
                            {
                                if (s.Equals(Web.WebSocketStatus.Disconnected))
                                    ws.Connect();
                            });
                    })
                    .Where(s => s.Equals(Web.WebSocketStatus.Connected))
                    .FirstAsync()
                    .Select(s => new Actions.DummyAction()) // to cast back to 'IRadixNodeAction'
                    .IgnoreElements();
        }
    }
}
