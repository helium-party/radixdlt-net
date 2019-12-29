namespace HeliumParty.RadixDLT.Actions
{
    /// <summary>
    /// A dispatchable action request for the node data of a given node
    /// </summary>
    public class GetNodeDataRequestAction : IJsonRpcMethodAction
    {
        public RadixNode Node { get; }

        public GetNodeDataRequestAction(RadixNode node)
        {
            Node = node ?? throw new System.ArgumentNullException(nameof(node));
        }

        public override string ToString() => "GET_NODE_DATA_REQUEST " + Node;
    }
}
