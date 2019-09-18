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
        
        public WebSocketClient GetOrCreate(RadixNode node)
        {
            throw new System.NotImplementedException();
        }

        public System.IObservable<RadixNode> GetNewNodes() => _NewNodes;
    }
}
