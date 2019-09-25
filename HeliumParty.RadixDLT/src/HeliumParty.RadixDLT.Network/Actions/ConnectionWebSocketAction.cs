namespace HeliumParty.RadixDLT.Actions
{
    /// <summary>
    /// A dispatchable action to request to start a websocket connection with a given node.
    /// </summary>
    public class ConnectionWebSocketAction : IRadixNodeAction
    {
        public RadixNode Node { get; }

        private ConnectionWebSocketAction(RadixNode node)
        {
            Node = node ?? throw new System.ArgumentNullException(nameof(node));
        }

        public static ConnectionWebSocketAction From(RadixNode node) => new ConnectionWebSocketAction(node);

        public override string ToString() => $"CONNECT_WEB_SOCKET_ACTION {Node}";
    }
}
