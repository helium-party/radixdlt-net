using System;
using System.Reactive.Linq;

namespace HeliumParty.RadixDLT.Epics
{
    /// <summary>
    /// Epic which emits atoms on a <see cref="Actions.FetchAtomsRequestAction"/> query forever until a <see cref="Actions.FetchAtomsCancelAction"/> occurs
    /// </summary>
    public class FetchAtomsEpic : IRadixNetworkEpic
    {
        private static readonly TimeSpan DelayClose = TimeSpan.FromMinutes(5);  // TODO: Make a constant?

        private readonly WebSockets _WebSockets;

        public FetchAtomsEpic(WebSockets webSockets)
        {
            _WebSockets = webSockets;
        }

        /// <summary>
        /// Observable to only wait until node is connected
        /// Note that the return collection won't (and shouldn't have to) provide any items
        /// </summary>
        private IObservable<Web.WebSocketStatus> WaitForConnection(RadixNode node)
        {
            Web.WebSocketClient ws = _WebSockets.GetOrCreate(node);
            return ws.State.Do(s =>
            {
                if (s.Equals(Web.WebSocketStatus.Disconnected))
                    ws.Connect();
            })
            .Where(s => s.Equals(Web.WebSocketStatus.Connected))
            .FirstAsync()
            .IgnoreElements();
        }

        private IObservable<IRadixNodeAction> FetchAtoms(Actions.FetchAtomsRequestAction request, RadixNode node)
        {
            Web.WebSocketClient ws = _WebSockets.GetOrCreate(node);
            var uuid = request.UUID;
            var address = request.Address;
            //var client = new 
            throw new NotImplementedException();
        }

        public IObservable<IRadixNodeAction> Epic(IObservable<IRadixNodeAction> actions, IObservable<RadixNetworkState> networkState)
        {
            throw new NotImplementedException();
        }
    }
}
