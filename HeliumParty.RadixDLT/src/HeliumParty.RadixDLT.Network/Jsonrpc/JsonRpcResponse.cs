using Newtonsoft.Json.Linq;

namespace HeliumParty.RadixDLT.Jsonrpc
{
    /// <summary>
    /// A container for a single Json Rpc response
    /// </summary>
    public class JsonRpcResponse
    {
        public bool WasSuccessful { get; private set; }
        public JObject Response { get; private set; }

        public JsonRpcResponse(bool wasSuccessful, JObject response)
        {
            WasSuccessful = wasSuccessful;
            Response = response ?? throw new System.ArgumentNullException(nameof(response));
        }

        // TODO: Not sure if this is what we want 
        public JToken GetResult() => Response.GetValue("response");
        public JToken GetError() => Response.GetValue("error");
    }
}
