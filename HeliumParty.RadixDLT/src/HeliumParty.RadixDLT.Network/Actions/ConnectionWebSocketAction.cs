namespace HeliumParty.RadixDLT.Actions
{
    /// <summary>
    /// A dispatchable action to request to start a websocket connection with a given node.
    /// </summary>
    public class ConnectionWebSocketAction : IRadixNodeAction           // TODO: Change to struct?
    {
        public RadixNode Node { get; }

        public ConnectionWebSocketAction(RadixNode node)
        {
            Node = node ?? throw new System.ArgumentNullException(nameof(node));
        }

        public override string ToString() => $"CONNECT_WEB_SOCKET_ACTION {Node}";
    }
}
