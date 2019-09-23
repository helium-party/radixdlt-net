namespace HeliumParty.RadixDLT.Actions
{
    /// <summary>
    /// A dispatchable action request for the node data of a given node
    /// </summary>
    public class GetNodeDataRequestAction : IRadixNodeAction
    {
        public RadixNode Node { get; }

        private GetNodeDataRequestAction(RadixNode node)
        {
            Node = node ?? throw new System.ArgumentNullException(nameof(node));
        }

        public static GetNodeDataRequestAction From(RadixNode node) => new GetNodeDataRequestAction(node);

        public override string ToString() => "GET_NODE_DATA_REQUEST " + Node;
    }
}
