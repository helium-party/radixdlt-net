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
        private readonly int _Version;
        private readonly RadixUniverseConfig _UniverseConfig;

        public RadixNodeState(RadixNode node, WebSocketStatus status, NodeRunnerData data,
            int version, RadixUniverseConfig universeConfig)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));
            if (status == null)
                throw new ArgumentNullException(nameof(status));
            
            _Node = node;
            _Status = status;
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

        public int GetVersion() => _Version;
        public NodeRunnerData GetData() => (_Data == null) ? default(_Data) : _Data;
        public WebSocketStatus GetStatus() => _Status;
        public RadixUniverseConfig GetUniverseConfig() => (_UniverseConfig == null) ? default(RadixUniverseConfig) : _UniverseConfig;


    }
}
