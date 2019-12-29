using System;

namespace HeliumParty.RadixDLT.Jsonrpc
{
    public interface IPersistentChannel
    {
        bool SendMessage(string mesage);
        IDisposable AddListener(System.Action<string> listener);
    }
}
