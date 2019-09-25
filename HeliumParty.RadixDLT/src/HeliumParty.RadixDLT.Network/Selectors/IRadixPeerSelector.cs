using System.Collections.Generic;

namespace HeliumParty.RadixDLT.Selectors
{
    /// <summary>
    /// Peer selector that selects single peer out of a list of available peers with at least one viable peer in it
    /// </summary>
    public interface IRadixPeerSelector 
    {
        /// <summary>
        /// A method that selects a single peer from a specified list of available peers
        /// </summary>
        /// <returns>The selected peer</returns>
        RadixNode Apply(List<RadixNode> nodes);
    }
}
