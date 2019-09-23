using HeliumParty.RadixDLT.Log;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using reactive_subjects = System.Reactive.Subjects;
using websockets = System.Net.WebSockets;

namespace HeliumParty.RadixDLT.Web
{
    public class WebSocketClient : Jsonrpc.IPersistentChannel
    {
        /// <summary>
        /// We might not need the logger directly at startup, therefore lazy loading it.
        /// </summary>
        private static OutputLogger _LazyLogger;
        private static OutputLogger _Logger
        {
            get
            {
                if (_LazyLogger == null)
                    _LazyLogger = new OutputLogger();
                return _LazyLogger;
            }
        }

        private readonly object _Lock = new object();

        public reactive_subjects::BehaviorSubject<WebSocketStatus> State { get; private set; } =
            new reactive_subjects::BehaviorSubject<WebSocketStatus>(WebSocketStatus.Disconnected);

        private readonly Func<websockets::WebSocket> _WebSocketFactory;
        private websockets::WebSocket _WebSocket;


        public WebSocketClient(Func<websockets::WebSocket> webSocketFactory)
        {
            _WebSocketFactory = webSocketFactory;

            // TODO: Java lib wants this disposed, lets see for their implementation
            State
                .Select(state => state.Equals(WebSocketStatus.Failed))
                .Throttle(TimeSpan.FromMinutes(1))
                .Subscribe(state =>
               {
                   lock (_Lock)
                   {
                       if (State.Value.Equals(WebSocketStatus.Failed))
                           State.OnNext(WebSocketStatus.Disconnected);
                   }
               });
        }

        private websockets::WebSocket ConnectWebSocket()
        {
            lock (_Lock)
            {

            }
        }

        /// <summary>
        /// Attempts to connect to this radix node if not already connected
        /// </summary>
        public void Connect()
        {
            lock (_Lock)
            {
                switch (State.Value)
                {
                    case WebSocketStatus.Connecting:
                        break;
                    case WebSocketStatus.Connected:
                        break;
                    case WebSocketStatus.Closing:
                        break;
                    case WebSocketStatus.Disconnected:
                    case WebSocketStatus.Failed:
                        State.OnNext(WebSocketStatus.Connecting);
                        this.ConnectWebSocket();
                        break;
                    default:
                        break;
                }
            }
        }
        
        public void AddListener(Action<string> listener)
        {


            throw new NotImplementedException();
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public bool Close()
        {
            throw new NotImplementedException();
        }

        public bool SendMessage(string message)
        {
            if (_Logger.IsDebugEnabled)
                _Logger.LogMessage($"Websocket {this.GetHashCode()} send: {message}");

            lock (_Lock)
            {
                if (State.Value.Equals(WebSocketStatus.Connected))
                    _Logger.LogMessage("Most likely a programming bug, shouldn't end here.", LogLevel.Critical);

            }
        }
    }
}
