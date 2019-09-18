namespace HeliumParty.RadixDLT.Jsonrpc
{
    public interface IPersistentChannel : ICancellable
    {
        bool SendMessage(string mesage);
        void AddListener(System.Action<string> listener);
    }
}
