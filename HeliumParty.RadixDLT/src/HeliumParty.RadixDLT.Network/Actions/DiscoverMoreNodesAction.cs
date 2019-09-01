namespace HeliumParty.RadixDLT.Actions
{
    /// <summary>
    /// A dispatchable action which requests for more nodes to be discovered
    /// </summary>
    public class DiscoverMoreNodesAction : IRadixNodeAction
    {
        public RadixNode Node => throw new System.InvalidOperationException("This action doesn't contain a node!");

        public override string ToString() => $"DISCOVER_MORE_NODES";
    }
}
