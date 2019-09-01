namespace HeliumParty.RadixDLT
{
    /// <summary>
    /// An action utilized in the Radix Network epics and reducers.
    /// </summary>
    public interface IRadixNodeAction
    {
        /// <summary>
        /// The node associated with the network action
        /// </summary>
        RadixNode Node { get; }
    }
}
