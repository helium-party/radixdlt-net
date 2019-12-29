using HeliumParty.RadixDLT.Web;
using System;
using System.Reactive.Linq;

namespace HeliumParty.RadixDLT.Epics
{
    public static class Utils
    {
        public static IObservable<WebSocketStatus> WaitForConnection(WebSockets webSockets, RadixNode node, out WebSocketClient connectedWebSocket)
        {
            var ws = webSockets.GetOrCreate(node);
            connectedWebSocket = ws;    // The webSocket as 'out' parameter cannot be used for the ws.Connect() - Compiler error
            return ws.State.Do(s =>
            {
                if (s.Equals(Web.WebSocketStatus.Disconnected))
                    ws.Connect();
            })
            .Where(s => s.Equals(Web.WebSocketStatus.Connected));
        }
    }
}
