using System;
using System.Reactive.Linq;

namespace HeliumParty.RadixDLT.Epics
{
    /// <summary>
    /// Epic which attempts to close a websocket when a Json Rpc method is finished executing.
    /// Note that the websocket won't close if there are still listeners
    /// </summary>
    public class RadixJsonRpcAutoCloseEpic : IRadixNetworkEpic
    {
        private static readonly TimeSpan DelayClose = TimeSpan.FromSeconds(5);
        private readonly WebSockets _WebSockets;

        public RadixJsonRpcAutoCloseEpic(WebSockets webSockets)
        {
            _WebSockets = webSockets ?? throw new ArgumentNullException(nameof(webSockets));
        }

        public IObservable<IRadixNodeAction> Epic(IObservable<IRadixNodeAction> actions, IObservable<RadixNetworkState> networkState)
        {
            return
                actions
                    .Where(a => Generics.InheritsOrIsInstance(a.GetType(), typeof(Actions.IJsonRpcResultAction<>)))
                    .Delay(DelayClose)
                    .Do(a => _WebSockets.GetOrCreate(a.Node).Close())
                    .IgnoreElements();
        }
    }
}
