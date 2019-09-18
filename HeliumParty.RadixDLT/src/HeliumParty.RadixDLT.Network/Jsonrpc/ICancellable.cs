namespace HeliumParty.RadixDLT.Jsonrpc
{
    /// <summary>
    /// This interface requires the user to provide a suitable cancellation method of its own operations
    /// </summary>
    public interface ICancellable
    {
        /// <summary>
        /// Contains class-specific implementation of how to cancel running operations
        /// </summary>
        void Cancel();
    }
}
