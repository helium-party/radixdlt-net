using System;

namespace HeliumParty.RadixDLT.Jsonrpc
{
    public interface IPersistentChannel : ICancellable
    {
        bool SendMessage(string mesage);
        IDisposable AddListener(System.Action<string> listener);
        void RemoveListener();
    }
}
