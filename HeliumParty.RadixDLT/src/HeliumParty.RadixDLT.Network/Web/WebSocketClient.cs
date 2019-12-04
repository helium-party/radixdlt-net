using HeliumParty.RadixDLT.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace HeliumParty.RadixDLT.Web
{
    public class WebSocketClient
    {
        #region Properties

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

        /// <summary>
        /// The listener actions that will be called when a new message is received
        /// </summary>
        private List<Action<string>> _Listeners = new List<Action<string>>();

        private readonly object _Lock = new object();

        /// <summary>
        /// The current connection state of the websocket client
        /// </summary>
        public BehaviorSubject<WebSocketStatus> State { get; private set; } =
            new BehaviorSubject<WebSocketStatus>(WebSocketStatus.Disconnected);

        /// <summary>
        /// The connection to the node
        /// </summary>
        WebSocketSharp.WebSocket _ConnectionSocket;        

        /// <summary>
        /// The target node for this client
        /// </summary>
        public RadixNode Node { get; }

        #endregion

        /// <summary>
        /// The constructor to initialize the state observable
        /// </summary>
        /// <param name="node">The node this <see cref="WebSocketClient"/> should connect to</param>
        public WebSocketClient(RadixNode node)
        {
            Node = node ?? throw new System.ArgumentNullException(nameof(node));
            
            // TODO: State is disposable, however it isn't implemented yet in Java.
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

        #region Public methods

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
                        SetUpConnection();
                        break;
                    default:
                        break;
                }
            }
        }
        
        /// <summary>
        /// Closes the connection to the node
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            lock (_Lock)
            {
                State.OnNext(WebSocketStatus.Closing);
                // Notify node that we are closing the connection regularly
                _ConnectionSocket.Close(5001);
            }

            return true;
        }

        /// <summary>
        /// Adds a listener action that will be called when a new message is received
        /// </summary>
        /// <param name="listener">The listener method that will be called, 
        /// the message will be its parameter</param>
        public void AddListener(Action<string> listener) => _Listeners.Add(listener);

        /// <summary>
        /// Removes the listener from the notification list
        /// </summary>
        /// <param name="listener">The listener to remove</param>
        public void RemoveListener(Action<string> listener) => _Listeners.Remove(listener);
        
        /// <summary>
        /// Sends a message to the node
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <returns>Whether sending was successful</returns>
        public bool SendMessage(string message)
        {
            if (_Logger.IsDebugEnabled)
                _Logger.LogMessage($"Websocket {this.GetHashCode()} send: {message}");

            lock (_Lock)
            {
                if (State.Value.Equals(WebSocketStatus.Connected))
                {
                    _Logger.LogMessage("Most likely a programming bug, shouldn't end here.", LogLevel.Critical);
                    return false;
                }

                var send_task = _Connection.Send(message);
                send_task.Wait();

                return !send_task.IsFaulted && !send_task.IsCanceled; // TODO: Check whether this actually tells us about successful sending
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Sets up the connection with all our required functionality
        /// </summary>
        private void SetUpConnection()
        {
            _ConnectionSocket = new WebSocketSharp.WebSocket(Node.SocketEndpoint.Host); // host is the whole uri (with 'wss:' and the port
            _ConnectionSocket.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;

            _ConnectionSocket.OnOpen += (sender, e) =>
            {
                if (_Logger.IsDebugEnabled)
                    _Logger.LogMessage($"Websocket {this.GetHashCode()} opened");

                State.OnNext(WebSocketStatus.Connected);
            };
            
            _ConnectionSocket.OnMessage += (sender, e) =>
            {
                // We only want to listen on test messages
                if (!e.IsText)
                    return;

                if (_Logger.IsDebugEnabled)
                    _Logger.LogMessage($"Websocket {this.GetHashCode()} message: {e.Data}");

                // Broadcast received message
                _Listeners.ForEach(a => a(e.Data));
            };

            _ConnectionSocket.OnClose += (sender, e) =>
            {
                if (_Logger.IsDebugEnabled)
                    _Logger.LogMessage($"Websocket {this.GetHashCode()} closed ({e.Code}/{e.Reason})");

                State.OnNext(WebSocketStatus.Disconnected);
            };

            _ConnectionSocket.OnError += (sender, e) =>
            {
                if (_Logger.IsDebugEnabled)
                {
                    var message_header = $"Websocket {this.GetHashCode()} failed:";
                    _Logger.LogMessage($"{message_header} exceptionType={e.GetType().ToString()}, message={e.Message}");
                }

                State.OnNext(WebSocketStatus.Disconnected);
            };

        }

        #endregion
    }
}
