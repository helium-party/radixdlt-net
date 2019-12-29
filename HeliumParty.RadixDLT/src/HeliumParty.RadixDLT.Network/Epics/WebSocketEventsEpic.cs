using System;
using System.Reactive.Linq;

namespace HeliumParty.RadixDLT.Epics
{
    /// <summary>
    /// Epic which emits websocket events for each new node
    /// </summary>
    public class WebSocketEventsEpic : IRadixNetworkEpic
    {
        private readonly WebSockets _WebSockets;

        public WebSocketEventsEpic(WebSockets webSockets)
        {
            _WebSockets = webSockets;
        }

        public IObservable<IRadixNodeAction> Epic(IObservable<IRadixNodeAction> actions, IObservable<RadixNetworkState> networkState)
        {
            return _WebSockets
                .GetNewNodes()
                .SelectMany(n => 
                    _WebSockets
                    .GetOrCreate(n)
                    .State
                    .Select(s => Actions.WebSocketEvent.NodeStatus(n, s))
                    );
        }
    }
}
