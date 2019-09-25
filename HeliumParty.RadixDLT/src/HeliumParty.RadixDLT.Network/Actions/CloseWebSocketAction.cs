namespace HeliumParty.RadixDLT.Actions
{
    /// <summary>
    /// A dispatchable action to request to close a websocket connection with a given node.
    /// </summary>
    public class CloseWebSocketAction : IRadixNodeAction            // TODO: Change to struct?
    {
        public RadixNode Node { get; }

        private CloseWebSocketAction(RadixNode node)
        {
            Node = node ?? throw new System.ArgumentNullException(nameof(node));
        }

        public static CloseWebSocketAction From(RadixNode node) => new CloseWebSocketAction(node);

        public override string ToString() => $"CLOSE_WEBSOCKET_ACTION {Node}";
    }
}
