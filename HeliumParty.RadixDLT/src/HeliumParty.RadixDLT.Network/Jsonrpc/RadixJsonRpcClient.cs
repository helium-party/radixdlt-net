using HeliumParty.RadixDLT.Log;
using Newtonsoft.Json.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace HeliumParty.RadixDLT.Jsonrpc
{
    public class RadixJsonRpcClient
    {
        /// <summary>
        /// We might not need the logger directly at startup, therefore lazy loading it.
        /// </summary>
        private static OutputLogger _LazyLogger;
        private static OutputLogger _Logger
        {
            get
            {
                if (_LazyLogger == null)
                    _LazyLogger = new OutputLogger();
                return _LazyLogger;
            }
        }

        #region Constants

        /// <summary>
        /// API version of client, must match with the node
        /// </summary>
        public const int ApiVersion = 1;

        private const int _DefaultTimeoutSec = 30;

        #endregion

        /// <summary>
        /// Cached API version of node
        /// </summary>
        private ReplaySubject<int> _NodeApiVersion = new ReplaySubject<int>(1);
        
        /// <summary>
        /// Cached universe configuration of node
        /// </summary>
        private ReplaySubject<Universe.RadixUniverseConfig> _NodeUniverseConfig = new ReplaySubject<Universe.RadixUniverseConfig>(1); 

        /// <summary>
        /// Performs the actual Json-Rpc call with the node, deserializes the recevied response
        /// </summary>
        /// <param name="method">The name of the method to call</param>
        /// <param name="call_params">Additional parameters for the method</param>
        /// <returns>The response of the node</returns>
        public Observable<JsonRpcResponse> JsonRpcCall(string method, JObject call_params)
        {
            return Observable.Create<JsonRpcResponse>(emitter =>
            {
                var callGuid = System.Guid.NewGuid();
                var requestObject = new JObject();

                requestObject.Add("id", callGuid);
                requestObject.Add("method", method);
                requestObject.Add("params", call_params);



            });
        }

    }
}
