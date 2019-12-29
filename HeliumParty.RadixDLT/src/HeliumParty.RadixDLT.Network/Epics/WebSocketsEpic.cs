using System;
using System.Linq;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace HeliumParty.RadixDLT.Epics
{
    /// <summary>
    /// Epic which manages the store of low level webSockets and connects epics dependent on usage of these webSockets
    /// </summary>
    public class WebSocketsEpic : IRadixNetworkEpic
    {
        private readonly List<Func<WebSockets, IRadixNetworkEpic>> _Epics = new List<Func<WebSockets, IRadixNetworkEpic>>();
        private readonly WebSockets _WebSockets;

        public WebSocketsEpic(WebSockets webSockets, List<Func<WebSockets, IRadixNetworkEpic>> epics)
        {
            _WebSockets = webSockets;
            _Epics = epics;
        }

        public IObservable<IRadixNodeAction> Epic(IObservable<IRadixNodeAction> actions, IObservable<RadixNetworkState> networkState)
        {
            return Observable.Merge(
                _Epics
                .Select(f => f(_WebSockets).Epic(actions, networkState)));
        }
    }
}
