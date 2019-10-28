using HeliumParty.RadixDLT.Web;
using System.Collections.Generic;

namespace HeliumParty.RadixDLT
{
    /// <summary>
    /// All websockets are created and managed here
    /// </summary>
    public class WebSockets
    {
        private readonly Dictionary<RadixNode, WebSocketClient> _WebSockets = new Dictionary<RadixNode, WebSocketClient>();
        private readonly System.Reactive.Subjects.ReplaySubject<RadixNode> _NewNodes = new System.Reactive.Subjects.ReplaySubject<RadixNode>();

        private readonly object _Lock = new object();
        
        /// <summary>
        /// Returns a unique websocket for a given node. Will never return null
        /// </summary>
        /// <param name="node">A radix node to get the websocket client for</param>
        /// <returns>A websocket client for the specified node</returns>
        public WebSocketClient GetOrCreate(RadixNode node)
        {
            lock (_Lock)
            {
                if (_WebSockets.ContainsKey(node))
                    return _WebSockets[node];

                var wsc = new Web.WebSocketClient(node);
                _WebSockets.Add(node, wsc);
                return wsc;
            }
        }

        public System.IObservable<RadixNode> GetNewNodes() => _NewNodes;
    }
}
