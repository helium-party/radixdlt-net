namespace HeliumParty.RadixDLT.Actions
{
    /// <summary>
    /// A dispatchable action to request to close a websocket connection with a given node.
    /// </summary>
    public class CloseWebSocketAction : IRadixNodeAction
    {
        public RadixNode Node { get; }

        public CloseWebSocketAction(RadixNode node)
        {
            Node = node ?? throw new System.ArgumentNullException(nameof(node));
        }

        public override string ToString() => $"CLOSE_WEBSOCKET_ACTION {Node}";
    }
}
