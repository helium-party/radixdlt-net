namespace HeliumParty.RadixDLT.Actions
{
    /// <summary>
    /// The result from executing a Json Rpc Method
    /// </summary>
    /// <typeparam name="T">Class of result</typeparam>
    public interface IJsonRpcResultAction<T> : IRadixNodeAction
    {
        T GetResult();
    }
}
