using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace HeliumParty.RadixDLT
{
    /// <summary>
    /// The current state of all the nodes we are connected to
    /// </summary>
    public class RadixNetworkState
    {
        public ImmutableDictionary<RadixNode, RadixNodeState> NodeStateCollection { get; }

        public RadixNetworkState(Dictionary<RadixNode, RadixNodeState> nodes)
        {
            if (nodes == null)
                throw new ArgumentNullException(nameof(nodes));

            NodeStateCollection = ImmutableDictionary.CreateRange(nodes);             
        }

        public HashSet<RadixNode> GetNodes() => new HashSet<RadixNode>(NodeStateCollection.Keys);

        public override string ToString() => NodeStateCollection.ToString();
    }
}
