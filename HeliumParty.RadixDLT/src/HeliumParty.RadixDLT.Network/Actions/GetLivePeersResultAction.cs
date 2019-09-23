using System.Collections.Generic;
using System.Linq;
using HeliumParty.RadixDLT.Jsonrpc;

namespace HeliumParty.RadixDLT.Actions
{
    /// <summary>
    /// A dispatchable result from a get live peers request
    /// </summary>
    public class GetLivePeersResultAction : IJsonRpcResultAction<List<NodeRunnerData>>
    {
        public RadixNode Node { get; }
        private readonly List<NodeRunnerData> _Data;

        private GetLivePeersResultAction(RadixNode node, List<NodeRunnerData> data)
        {
            if (data == null)
                throw new System.ArgumentNullException(nameof(data));

            Node = node ?? throw new System.ArgumentNullException(nameof(node));

            _Data = new List<NodeRunnerData>(data); // This doesn't create a shallow / deep copy of the class instances themself
        }

        public static GetLivePeersResultAction From(RadixNode node, List<NodeRunnerData> data) => new GetLivePeersResultAction(node, data);

        public List<NodeRunnerData> GetResult() => _Data.ToList();

        public override string ToString() => $"GET_LIVE_PEERS_RESULT {Node} {_Data.Count} peers"; 
    }
}
