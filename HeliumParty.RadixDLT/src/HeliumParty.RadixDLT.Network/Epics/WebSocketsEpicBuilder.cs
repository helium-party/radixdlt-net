using System;
using System.Collections.Generic;

namespace HeliumParty.RadixDLT.Epics
{
    /// <summary>
    /// Builds a <see cref="WebSocketsEpic"/> composed of epics which require <see cref="WebSockets"/>. After being built, 
    /// all epics share the same set of webSockets which can be used.
    /// </summary>
    public class WebSocketsEpicBuilder
    {
        private List<Func<WebSockets, IRadixNetworkEpic>> _Epics = new List<Func<WebSockets, IRadixNetworkEpic>>();
        private WebSockets _WebSockets;

        public WebSocketsEpicBuilder Add(Func<WebSockets, IRadixNetworkEpic> epic)
        {
            _Epics.Add(epic);
            return this;
        }

        public WebSocketsEpicBuilder SetWebSockets(WebSockets webSockets)
        {
            _WebSockets = webSockets;
            return this;
        }

        public WebSocketsEpic Build()
        {
            if (_WebSockets == null)
                throw new InvalidOperationException($"WebSockets is not set!");

            return new WebSocketsEpic(_WebSockets, _Epics);
        }
    }
}
