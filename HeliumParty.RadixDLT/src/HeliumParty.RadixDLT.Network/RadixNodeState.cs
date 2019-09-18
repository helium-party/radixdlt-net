using HeliumParty.RadixDLT.Jsonrpc;
using HeliumParty.RadixDLT.Web;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT
{
    /// <summary>
    /// Immutable state at a certain point in time of a <see cref="RadixNode"/>
    /// </summary>
    public class RadixNodeState
    {
        private readonly RadixNode _Node;
        private readonly NodeRunnerData _Data;

        public Address.RadixUniverseConfig UniverseConfig { get; }
        public WebSocketStatus Status { get; }
        public int? Version { get; }

        public RadixNodeState(RadixNode node, WebSocketStatus status, NodeRunnerData data,
            int? version, Address.RadixUniverseConfig universeConfig)
        {
            _Node = node ?? throw new ArgumentNullException(nameof(node));
            Status = status;
            _Data = data;
            Version = version;
            UniverseConfig = universeConfig;
        }

        public static RadixNodeState From(RadixNode node, WebSocketStatus status)
        {
            return new RadixNodeState(node, status, null, null, null);
        }

        public static RadixNodeState From(RadixNode node, WebSocketStatus status, NodeRunnerData data)
        {
            return new RadixNodeState(node, status, data, null, null);
        }

        public static RadixNodeState From(RadixNode node, WebSocketStatus status, NodeRunnerData data, Address.RadixUniverseConfig universeConfig)
        {
            return new RadixNodeState(node, status, data, null, universeConfig);
        }

        public RadixNode GetNode() => _Node;

        public NodeRunnerData GetData() => _Data ?? default(NodeRunnerData);

        public ShardSpace GetShards() => _Data.GetShards();
    }
}
