using HeliumParty.RadixDLT.Reducers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace HeliumParty.RadixDLT
{
    /// <summary>
    /// Service that builds the <see cref="RadixNetworkController"/>
    /// </summary>          
    public class RadixNetworkControllerBuilder  // TODO: Check where we want to have the logic for class-builders
    {
        private RadixNetwork _Network;
        private readonly ImmutableList<IRadixNetworkEpic>.Builder _Epics = ImmutableList.CreateBuilder<IRadixNetworkEpic>();
        private readonly ImmutableList<Action<IRadixNodeAction>>.Builder _Reducers = ImmutableList.CreateBuilder<Action<IRadixNodeAction>>();
        private readonly HashSet<RadixNode> _InitialNodes = new HashSet<RadixNode>();

        public RadixNetworkControllerBuilder AddReducer(Action<IRadixNodeAction> reducer)
        {
            _Reducers.Add(reducer);
            return this;
        }

        public RadixNetworkControllerBuilder SetNetwork(RadixNetwork network)
        {
            _Network = network;
            return this;
        }

        public RadixNetworkControllerBuilder AddEpic(IRadixNetworkEpic epic)
        {
            _Epics.Add(epic);
            return this;
        }

        public RadixNetworkControllerBuilder AddInitialNodes(HashSet<RadixNode> nodes)
        {
            foreach (var node in nodes)
                _InitialNodes.Add(node);
            return this;
        }

        public RadixNetworkControllerBuilder AddInitialNode(RadixNode node)
        {
            _InitialNodes.Add(node);
            return this;
        }

        public RadixNetworkController Build()
        {
            if (_Network == null)
                _Network = new RadixNetwork();

            Dictionary<RadixNode, RadixNodeState> initStates = new Dictionary<RadixNode, RadixNodeState>();
            foreach (var node in _InitialNodes)
                initStates.Add(node, new RadixNodeState(node, Web.WebSocketStatus.Disconnected));

            return new RadixNetworkController(
                _Network,
                new RadixNetworkState(initStates),
                _Epics.ToImmutable(),
                _Reducers.ToImmutable()
                );
        }
    }
}
