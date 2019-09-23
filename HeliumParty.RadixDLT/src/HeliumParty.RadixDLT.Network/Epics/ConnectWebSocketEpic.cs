using System;
using System.Reactive.Linq;

namespace HeliumParty.RadixDLT.Epics
{
    /// <summary>
    /// Epic which begins and closes a websocket connection when the respective actions 
    /// <see cref="Actions.ConnectionWebSocketAction"/> and <see cref="Actions.CloseWebSocketAction"/> are seen
    /// </summary>
    public class ConnectWebSocketEpic : IRadixNetworkEpic
    {
        private readonly WebSockets _WebSockets;

        public ConnectWebSocketEpic(WebSockets webSockets)
        {
            _WebSockets = webSockets;
        }

        public IObservable<IRadixNodeAction> Epic(IObservable<IRadixNodeAction> actions, IObservable<RadixNetworkState> networkState)
        {
            IObservable<IRadixNodeAction> onConnect =
                actions
                .Where(action => action is Actions.ConnectionWebSocketAction)
                .Do(connectAction => _WebSockets.GetOrCreate(connectAction.Node).Connect())
                .IgnoreElements()
                .AsObservable();    // TODO : AsObservable OK?

            IObservable<IRadixNodeAction> onClose =
                actions
                .Where(action => action is Actions.CloseWebSocketAction)
                .Do(closeAction => _WebSockets.GetOrCreate(closeAction.Node).Close())
                .IgnoreElements()
                .AsObservable();

            return Observable.Merge(onConnect, onClose);
        }
    }
}
