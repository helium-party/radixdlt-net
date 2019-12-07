using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Log;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
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
        public const int ClientApiVersion = 1;

        #endregion

        #region Private members

        private static readonly TimeSpan _DefaultTimeout = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Cached API version of node
        /// </summary>
        private BehaviorSubject<int> _NodeApiVersion = new BehaviorSubject<int>(0);

        /// <summary>
        /// Cached universe configuration of node
        /// </summary>
        private BehaviorSubject<Universe.RadixUniverseConfig> _NodeUniverseConfig = new BehaviorSubject<Universe.RadixUniverseConfig>(null);

        private IPersistentChannel _Channel;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor that fetches the servers api version as well as the universe configuration
        /// </summary>
        /// <param name="channel"></param>
        public RadixJsonRpcClient(IPersistentChannel channel)
        {
            _Channel = channel;

            JsonRpcCall("Api.getVersion")
                .Select(r => r.GetResult())
                .Select(r => r?.Value<int>("version") ?? ClientApiVersion)  //TODO: Check for changes in java lib at a later date
                .Do(v => _NodeApiVersion.OnNext(v));

            JsonRpcCall("Universe.getUniverse")
                .Select(r => r.GetResult())
                .Select(r => r?.ToObject<Universe.RadixUniverseConfig>())
                .Do(config => _NodeUniverseConfig.OnNext(config));
        }

        #endregion

        /// <summary>
        /// Performs a Json-Rpc call without any additional parameters with the node, 
        /// deserializes the recevied response
        /// </summary>
        /// <param name="method">The name of the method to call</param>
        /// <returns>The response of the node</returns>
        public IObservable<JsonRpcResponse> JsonRpcCall(string method)
        {
            return JsonRpcCall(method, new JObject());
        }

        /// <summary>
        /// Performs the actual Json-Rpc call with the node, deserializes the recevied response
        /// </summary>
        /// <param name="method">The name of the method to call</param>
        /// <param name="call_params">Additional parameters for the method</param>
        /// <returns>The response of the node</returns>
        public IObservable<JsonRpcResponse> JsonRpcCall(string method, JObject call_params)
        {
            if (String.IsNullOrEmpty(method))
                throw new ArgumentNullException(nameof(method));
            if (call_params == null)
                throw new ArgumentNullException(nameof(call_params));

            return Observable.Create<JsonRpcResponse>(emitter =>
            {
                var callGuid = System.Guid.NewGuid().ToString();

                // Prepare data to send
                var requestObject = new JObject
                {
                    { "id", callGuid },
                    { "method", method },
                    { "params", call_params }
                };

                // Listen for response
                _Channel.AddListener(msg =>
                {
                    var json = JsonConvert.DeserializeObject<JObject>(msg);
                    if (!json.ContainsKey("id"))
                        return;

                    var responseId = json.Value<string>("id");

                    if (String.IsNullOrEmpty(responseId) || !responseId.Equals(callGuid))
                        return;

                    emitter.OnNext(new JsonRpcResponse(json));
                });

                var sendSuccessful = _Channel.SendMessage(requestObject.ToString());
                if (sendSuccessful)
                    emitter.OnError(new System.SystemException($"Could not send message: {method} {call_params}"));

                return Disposable.Empty;    // TODO: Might need to create a real disposable
            }).Timeout(_DefaultTimeout);
        }

        /// <summary>
        /// Retrieves the api version the node supports, the result is cached for future calls
        /// </summary>
        /// <returns>The nodes api version</returns>
        public IObservable<int> GetNodeApiVersion() => _NodeApiVersion.AsObservable();

        /// <summary>
        /// Retrieves the universe the node is supporting, the result is cached for future calls
        /// </summary>
        /// <returns>The universe config the node is supporting</returns>
        public IObservable<Universe.RadixUniverseConfig> GetUniverseConfig() => _NodeUniverseConfig.AsObservable();

        /// <summary>
        /// Retrieves the data of the node we are connected to
        /// </summary>
        /// <returns>The data of the node we are connected to</returns>
        public IObservable<NodeRunnerData> GetNodeData()
        {
            return JsonRpcCall("Network.getInfo")
                .Select(r => r.GetResult())
                .Select(r => r?.ToObject<RadixSystem>())    // TODO: Serialization needs to be checked for this 
                .Select(sys => new NodeRunnerData(sys));
        }
        
        /// <summary>
        /// Retrieves a list of nodes the node we are connected to knows about
        /// </summary>
        /// <returns>A list of nodes in the same network</returns>
        public IObservable<List<NodeRunnerData>> GetLivePeers()
        {
            return JsonRpcCall("Network.getLivePeers")
                .Select(r => r.GetResult())
                .Select(r => r?.ToObject<List<NodeRunnerData>>())    // TODO: Serialization needs to be checked for this 
        }

        /// <summary>
        /// Submits an atom to the node
        /// </summary>
        /// <param name="atom">The atom to submit</param>
        /// <returns>An <see cref="IObservable{T}"/> that only leaves the termination message when atom is queued</returns>
        /// <exception cref="SubmitAtomException">Thrown in case submitting the atom went wrong</exception>
        public IObservable<object> PushAtom(Atom atom)
        {
            // TODO: Correct serialization of atom
            var jsonAtom = (JObject)JToken.FromObject(atom);
            return JsonRpcCall("Atoms.submitAtom", jsonAtom)
                .Select(r =>
                {
                    if (!r.WasSuccessful)
                        throw new SubmitAtomException(atom, (JObject)r.GetError());
                    return r;
                }
                ).IgnoreElements();
        }

        /// <summary>
        /// Sends a request to receive streaming updates on an atom's status.
        /// </summary>
        /// <param name="subscriberId">The subscriber id for the streaming updates</param>
        /// <param name="aid">the <see cref="AID"/> of the atom</param>
        /// <returns>An <see cref="IObservable{T}"/> that only leaves the termination message when message call was accepted</returns>
        /// <exception cref="JsonRpcCallException">In case the call was not successful</exception>
        public IObservable<object> RequestAtomStatusNotifications(string subscriberId, AID aid)
        {    
            var call_params = new JObject
            {
                { "aid", aid.ToString() },
                { "subscriberId", subscriberId }
            };

            return JsonRpcCall("Atoms.getAtomStatusNotifications", call_params).CheckCallSuccess();
        }

        /// <summary>
        /// Closes a streaming status subscription
        /// </summary>
        /// <param name="subscriberId">The subscriber id the stream should be closed for</param>
        /// <returns>An <see cref="IObservable{T}"/> that only leaves the termination message when message call was accepted</returns>
        /// <exception cref="JsonRpcCallException">In case the call was not successful</exception>
        public IObservable<object> CloseAtomStatusNotifications(string subscriberId)
        {
            var call_params = new JObject
            {
                { "subscriberId", subscriberId }
            };

            return JsonRpcCall("Atoms.closeAtomStatusNotifications", call_params).CheckCallSuccess();
        }

        // CHECK WHEN REBASE
        public IObservable<object> ObserveAtomStatusNotifications(string subscriberId)
        {
            throw new NotImplementedException();    // TODO: Missing AtomStatusNotification, check when rebased
        }

        /// <summary>
        /// Get the current status of an atom for this node
        /// </summary>
        /// <param name="aid">The <see cref="AID"/> of the atom</param>
        /// <returns>The status of the atom</returns>
        public IObservable<AtomStatus> GetAtomStatus(AID aid)
        {
            var call_params = new JObject
            {
                { "aid", aid.ToString() },
            };

            return JsonRpcCall("Atoms.getAtomStatus", call_params)
                .Select(r => r.GetResult())
                .Select(r => r?.Value<int>("status"))
                .Select(status_code =>(AtomStatus)(status_code ?? 0));  // In case response was null, we assume that the atom doesn't exist
        }

        /// <summary>
        /// Queries for an <see cref="Atom"/> by HID/>
        /// If the node does not carry the <see cref="Atom"/> (e.g. if it does not reside on the same shard),
        /// then this method will return null
        /// </summary>
        /// <param name="hid">The hash id of the atom being queried</param>
        /// <returns>The <see cref="Atom"/> if found, else null</returns>
        public IObservable<Atom> GetAtom(EUID hid)
        {
            var call_params = new JObject
            {
                { "hid", hid.ToString() },
            };

            return JsonRpcCall("Ledger.getAtoms", call_params)
                .Select(r => r.GetResult())
                .Select(r => r?.ToObject<List<Atom>>())    // TODO: Serialization needs to be checked for this
                .Select(atom_list => atom_list == null || atom_list.Count == 0 ? null : atom_list[0]);
        }

        // CHECK WHEN REBASE
        public IObservable<JsonRpcNotification<object>> ObserveNotifications(string subscriberId)
        {
            throw new NotImplementedException();    // TODO: Missing AtomStatusNotification, check when rebased
        }

        // CHECK WHEN REBASE
        public IObservable<JsonRpcNotification<object>> ObserveAtoms(string subscriberId)
        {
            throw new NotImplementedException();    // TODO: Missing AtomStatusNotification, check when rebased
        }

        public IObservable<object> CancelAtomsSubscribe(string subscriberId)
        {
            var call_params = new JObject
            {
                { "subscriberId", subscriberId }
            };

            return JsonRpcCall("Atoms.cancel", call_params).CheckCallSuccess();
        }
        
        public IObservable<object> SendAtomsSubscribe(string subscriberId, RadixAddress address)
        {
            var call_params = new JObject
            {
                { "query", new JObject { "address", address.ToString() } },
                { "subscriberId", subscriberId }
            };

            return JsonRpcCall("Atoms.subscribe", call_params).CheckCallSuccess();
        }
    }
}
