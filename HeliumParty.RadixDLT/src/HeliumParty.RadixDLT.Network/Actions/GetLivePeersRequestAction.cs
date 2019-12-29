namespace HeliumParty.RadixDLT.Actions
{
    /// <summary>
    /// A dispatchable action request for the live peers of a node
    /// </summary>
    public class GetLivePeersRequestAction : IJsonRpcMethodAction
    {
        public RadixNode Node { get; }

        public GetLivePeersRequestAction(RadixNode node)
        {
            Node = node ?? throw new System.ArgumentNullException(nameof(node));
        }
        
        public override string ToString() => "GET_LIVE_PEERS_REQUEST " + Node;
    }
}
