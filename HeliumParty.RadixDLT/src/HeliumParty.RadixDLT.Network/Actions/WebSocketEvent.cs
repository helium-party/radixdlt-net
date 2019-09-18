using HeliumParty.RadixDLT.Web;

namespace HeliumParty.RadixDLT.Actions
{
    /// <summary>
    /// A dispatchable event action signifying an event which has occurred to a websocket.
    /// </summary>
    public class WebSocketEvent : IRadixNodeAction
    {
        public enum WebSocketEventType // TODO: Wanna move it into an extra file? 
        {
            None,
            Connecting,
            Connected,
            Closing,
            Disconnected,
            Failed
        }

        public WebSocketEventType Type { get; }
        public RadixNode Node { get; }

        public WebSocketEvent(WebSocketEventType type, RadixNode node)
        {
            Node = node ?? throw new System.ArgumentNullException(nameof(node));
            Type = type;
        }

        /// <summary>
        /// Creates a new instance of <see cref="WebSocketEvent"/> with the specified node
        /// and the specified <see cref="WebSocketStatus"/> conferted into a <see cref="WebSocketEventType"/>
        /// </summary>
        /// <param name="node">Node to use for <see cref="WebSocketEvent"/></param>
        /// <param name="status">The status that will first be converted to a<see cref="WebSocketEventType"/></param>
        /// <returns>The newly created instance of <see cref="WebSocketEvent"/></returns>
        public static WebSocketEvent NodeStatus(RadixNode node, WebSocketStatus status)
        {
            System.Enum.TryParse<WebSocketEventType>(status.ToString(), out var convStatus);
            return new WebSocketEvent(convStatus, node);
        }

        public override string ToString() => $"WEBSOCKET_EVENT({Type}) {Node}";
    }
}
