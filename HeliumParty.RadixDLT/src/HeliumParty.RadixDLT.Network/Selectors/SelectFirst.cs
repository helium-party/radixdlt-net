using System.Collections.Generic;
using System.Linq;

namespace HeliumParty.RadixDLT.Selectors
{
    /// <summary>
    /// Selector that always returns the first node
    /// </summary>
    public static class SelectFirst
    {
        [System.Obsolete("Use First<> of System.Linq instead")]
        /// <summary>
        /// Applies the selector
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static RadixNode Apply(List<RadixNode> nodes)
        {
            if (nodes == null)
                throw new System.ArgumentNullException(nameof(nodes));
            if (nodes.Count == 0)
                throw new System.ArgumentException(nameof(nodes) + " must contain at least one element!");

            return nodes[0];
        }
    }
}
