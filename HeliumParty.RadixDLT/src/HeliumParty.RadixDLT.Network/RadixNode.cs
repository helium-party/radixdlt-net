namespace HeliumParty.RadixDLT
{
    public class RadixNode
    {
        private readonly string _Location;
        private readonly string _WebSocketUrl;
        private readonly string _HttpUrl;
        private readonly int _Port;
        private System.Net.DnsEndPoint _SocketEndpoint;

        #region Public members

        public System.Net.DnsEndPoint SocketEndpoint
        {
            get
            {
                if (_SocketEndpoint == null)
                    _SocketEndpoint = new System.Net.DnsEndPoint(_WebSocketUrl, _Port);

                return _SocketEndpoint;
            }
        }
        public bool IsSSL { get; }
        
        #endregion

        public RadixNode(string location, int port, bool useSSL = true)
        {
            _Location = location;
            _WebSocketUrl = (useSSL ? "wss://" : "ws://") + $"{location}:{port}/rpc";
            _HttpUrl = (useSSL ? "https://" : "http://") + $"{location}:{port}";
            _Port = port;

            IsSSL = useSSL;
        }
        
        public System.Net.DnsEndPoint GetHttpEndpoint(string path) => new System.Net.DnsEndPoint(CombineUrl(_HttpUrl, path), _Port);

        private static string CombineUrl(string url1, string url2)
        {
            // TODO: Lets unit test this

            if (string.IsNullOrEmpty(url1) || string.IsNullOrEmpty(url2))
                return string.IsNullOrEmpty(url1) ? string.IsNullOrEmpty(url2) ? string.Empty : url2 : url1;

            var separationCount = 0;
            if (url1.EndsWith("/"))
                separationCount++;
            if (url2.StartsWith("/"))
                separationCount++;

            if (separationCount == 1)
                return url1 + url2;

            else if (separationCount == 2)
                return url1 + url2.Substring(1);

            return url1 + "/" + url2;
        }

        private static string CombineUrl(params string[] urls)
        {
            if (urls.Length == 0)
                return string.Empty;
            if (urls.Length == 1)
                return urls[0];

            string finalUrl = urls[0];
            for (int i = 1; i < urls.Length; i++)
                finalUrl = CombineUrl(finalUrl, urls[i]);

            return finalUrl;
        }

        public override string ToString() => $"{_Location}:{_Port}";
        public override int GetHashCode() => (_WebSocketUrl + _Port.ToString()).GetHashCode();
        public override bool Equals(object obj)
        {
            if (obj is RadixNode node)
            {
                if (this._Location.Equals(node._Location)
                    && this._Port.Equals(node._Port))
                    return true;
            }

            return false;
        }
    }
}
