using HeliumParty.RadixDLT.Jsonrpc;
using System;

namespace HeliumParty.RadixDLT.Actions
{
    /// <summary>
    /// A dispatchable action to request to add a node to the network state
    /// </summary>
    public class AddNodeAction : IRadixNodeAction            // TODO: Change to struct?
    {
        /// <summary>
        /// The node assosiated with the network action
        /// </summary>
        public RadixNode Node { get; }
        public NodeRunnerData Data { get; }

        public AddNodeAction(RadixNode node) : this(node, null) { }

        public AddNodeAction(RadixNode node, NodeRunnerData data)
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
            Data = data;
        }

        public override string ToString() => $"ADD_NODE_ACTION {Node} {Data}";
    }
}
