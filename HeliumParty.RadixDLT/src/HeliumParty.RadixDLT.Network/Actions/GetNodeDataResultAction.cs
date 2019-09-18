using HeliumParty.RadixDLT.Jsonrpc;

namespace HeliumParty.RadixDLT.Actions
{
    /// <summary>
    /// A dispatchable action response for a given node data request
    /// </summary>
    public class GetNodeDataResultAction : IJsonRpcResultAction<NodeRunnerData>
    {
        public RadixNode Node { get; }
        private readonly NodeRunnerData _Data;

        private GetNodeDataResultAction(RadixNode node, NodeRunnerData data)
        {
            Node = node ?? throw new System.ArgumentNullException(nameof(node));
            _Data = data ?? throw new System.ArgumentNullException(nameof(data));
        }

        public GetNodeDataResultAction From(RadixNode node, NodeRunnerData data)
        {
            return new GetNodeDataResultAction(node, data);
        }

        public NodeRunnerData GetResult() => _Data;

        public override string ToString() => $"GET_NODE_DATA_RESPONSE {Node} {_Data}";
    }
}
