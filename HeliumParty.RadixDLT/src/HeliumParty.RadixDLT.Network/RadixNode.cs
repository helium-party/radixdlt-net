using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT
{
    public class RadixNode
    {
        private string _Location { get; }
        private string _WebSocketUrl { get; }
        private string _HttpUrl { get; }

        #region Private members

        #endregion

        #region Public members

        public int Port { get; }
        public Request WebSocketEndpoint { get; }
        public bool IsSSL { get; }
        
        #endregion

        public RadixNode(Request rootRequestLocation)
        {

        }

        public RadixNode(string location, int port, bool useSSL = true)
        {
            _Location = location;
            Port = port;
            IsSSL = useSSL;
            _WebSocketUrl = (useSSL ? "wss://" : "ws://") + $"{location}:{port}/rpc";
            _HttpUrl = (useSSL ? "https://" : "http://") + $"{location}:{port}";

            // TODO Build endpoint
        }

        public Request GetWebSocketEndpoint() => WebSocketEndpoint;
        public int GetPort() => Port;
        //public bool 
    }
}
