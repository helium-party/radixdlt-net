using HeliumParty.RadixDLT.Jsonrpc;
using HeliumParty.RadixDLT.Universe;
using HeliumParty.RadixDLT.Web;
using System;

namespace HeliumParty.RadixDLT
{
    /// <summary>
    /// Immutable state at a certain point in time of a <see cref="RadixNode"/>
    /// </summary>
    public class RadixNodeState
    {
        public RadixNode Node { get; }

        /// <summary>
        /// Status of <see cref="RadixNode"/>'s client
        /// </summary>
        public WebSocketStatus Status { get; }

        /// <summary>
        /// Node runner data of <see cref="RadixNode"/>
        /// </summary>
        public NodeRunnerData Data { get; }

        /// <summary>
        /// API version of <see cref="RadixNode"/>'s client
        /// </summary>
        public int Version { get; }

        /// <summary>
        /// Universe configuration of <see cref="RadixNode"/>'s client
        /// </summary>
        public RadixUniverseConfig UniverseConfig { get; }

        public ShardSpace Shards => Data.Shards;

        public RadixNodeState(
            RadixNode node,
            WebSocketStatus status) : this(node, status, null, null, -1) { }

        public RadixNodeState(
            RadixNode node,
            WebSocketStatus status,
            NodeRunnerData data) : this(node, status, data, null, -1) { }

        public RadixNodeState(
            RadixNode node,
            WebSocketStatus status,
            NodeRunnerData data,
            RadixUniverseConfig universeConfig) : this(node, status, data, universeConfig, -1) { }

        public RadixNodeState(
            RadixNode node, 
            WebSocketStatus status, 
            NodeRunnerData data,
            RadixUniverseConfig universeConfig,
            int version)
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
            Status = status;
            Data = data;
            UniverseConfig = universeConfig;
            Version = version;
        }

        public override string ToString() 
            => "RadixNodeState{" 
            + $"node='{Node}', status={Status}, data={Data}, universeConfig={UniverseConfig}" 
            + "}";

        public override bool Equals(object obj)
        {
            if (obj is RadixNodeState rns)
            {
                return this.Node.Equals(rns.Node) 
                    && this.Status.Equals(rns.Status)
                    && this.Data.Equals(rns.Data)
                    && this.Version.Equals(rns.Version)
                    && this.UniverseConfig.Equals(rns.UniverseConfig);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.Node.GetHashCode() * 3
                + (int)this.Status * 11
                + this.Data.GetHashCode() * 17
                + this.Version * 23
                + this.UniverseConfig.GetHashCode() * 31;
        }
    }
}
