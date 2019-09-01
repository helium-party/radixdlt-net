namespace HeliumParty.RadixDLT.Actions
{
    /// <summary>
    /// A dispatch action request for a connected node with some given shards
    /// </summary>
    public interface IFindANodeRequestAction : IRadixNodeAction
    {
        /// <summary>
        /// A shard space which must be intersected with a node's shard space to be selected
        /// </summary>
        System.Collections.Generic.HashSet<long> Shards { get; }
    }
}
