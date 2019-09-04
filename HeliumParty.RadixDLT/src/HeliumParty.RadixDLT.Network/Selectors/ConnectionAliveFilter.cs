namespace HeliumParty.RadixDLT.Selectors
{
    /// <summary>
    /// A connection status filter taht filters out inactive peers
    /// </summary>
    public class ConnectionAliveFilter
    {
        public bool Test(RadixNodeState nodeState)
        {
            return nodeState.Status ==
        }
        
    }
}
