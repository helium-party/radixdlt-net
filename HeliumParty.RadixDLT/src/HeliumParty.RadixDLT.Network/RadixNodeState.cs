using HeliumParty.RadixDLT.Jsonrpc;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT
{
    public class RadixNodeState
    {
        private readonly RadixNode _Node;
        private readonly WebSocketStatus _Status;
        private readonly NodeRunnerData _Data;
        private readonly int? _Version;
        private readonly RadixUniverseConfig _UniverseConfig;

        public RadixNodeState(RadixNode node, WebSocketStatus status, NodeRunnerData data,
            int? version, RadixUniverseConfig universeConfig)
        {
            _Node = node ?? throw new ArgumentNullException(nameof(node));
            _Status = status ?? throw new ArgumentNullException(nameof(status));
            _Data = data;
            _Version = version;
            _UniverseConfig = universeConfig;
        }

        public static RadixNodeState From(RadixNode node, WebSocketStatus status)
        {
            return new RadixNodeState(node, status, null, null, null);
        }

        public static RadixNodeState From(RadixNode node, WebSocketStatus status, NodeRunnerData data)
        {
            return new RadixNodeState(node, status, data, null, null);
        }

        public static RadixNodeState From(RadixNode node, WebSocketStatus status, NodeRunnerData data, RadixUniverseConfig universeConfig)
        {
            return new RadixNodeState(node, status, data, null, universeConfig);
        }

        public int? GetVersion() => _Version;
        public NodeRunnerData GetData() => _Data ?? default(NodeRunnerData);
        public WebSocketStatus GetStatus() => _Status;
        public RadixUniverseConfig GetUniverseConfig() => _UniverseConfig ?? default(RadixUniverseConfig);


    }
}
