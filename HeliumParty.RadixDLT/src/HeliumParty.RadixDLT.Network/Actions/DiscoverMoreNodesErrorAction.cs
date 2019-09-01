namespace HeliumParty.RadixDLT.Actions
{
    /// <summary>
    /// Action which represents an error which occurred when trying to discover more nodes
    /// </summary>
    public class DiscoverMoreNodesErrorAction : IRadixNodeAction
    {
        public RadixNode Node => throw new System.InvalidOperationException("This action doesn't contain a node!");     // TODO : Might want to change the 
                                                                                                                        // interface inheritance so that we 
                                                                                                                        // don't always have to implement this...

        public System.Exception Reason { get; }

        public DiscoverMoreNodesErrorAction(System.Exception reason) { Reason = reason; }

        public override string ToString() => $"DISCOVER_MORE_NODES";
    }
}
