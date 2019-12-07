using Newtonsoft.Json.Linq;

namespace HeliumParty.RadixDLT.Jsonrpc
{
    /// <summary>
    /// A container for a single Json Rpc response
    /// </summary>
    public class JsonRpcResponse
    {
        public bool WasSuccessful => IsResult && !IsError;
        public JObject Response { get; }
        public bool IsResult => Response.ContainsKey("result");
        public bool IsError => Response.ContainsKey("error");

        public JsonRpcResponse(JObject response)
        {
            Response = response ?? throw new System.ArgumentNullException(nameof(response));
        }

        /// <summary>
        /// Returns the 'result' property of the response
        /// </summary>
        /// <returns>The 'result' property, null if it doesn't exist</returns>
        public JToken GetResult() => Response.GetValue("result");

        /// <summary>
        /// Returns the 'error' property of the response
        /// </summary>
        /// <returns>The 'error' property, null if it doesn't exist</returns>
        public JToken GetError() => Response.GetValue("error");
    }
}
