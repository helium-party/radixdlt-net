using System.Collections.Generic;

namespace HeliumParty.RadixDLT.Selectors
{
    /// <summary>
    /// A simple randomized selector that returns an arbitrary peer out of the specified list
    /// </summary>
    public class RandomSelector : IRadixPeerSelector
    {
        private System.Random _Random = new System.Random();
        public RadixNode Apply(List<RadixNode> nodes) => nodes[_Random.Next(nodes.Count)];
    }
}
