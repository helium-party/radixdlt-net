namespace HeliumParty.RadixDLT.Actions
{
    /// <summary>
    /// A dispatchable action request for the live peers of a node
    /// </summary>
    public class GetLivePeersRequestAction : IRadixNodeAction
    {
        public RadixNode Node { get; }

        private GetLivePeersRequestAction(RadixNode node)
        {
            Node = node ?? throw new System.ArgumentNullException(nameof(node));
        }

        public static GetLivePeersRequestAction From(RadixNode node) => new GetLivePeersRequestAction(node);

        public override string ToString() => "GET_LIVE_PEERS_REQUEST " + Node;
    }
}
