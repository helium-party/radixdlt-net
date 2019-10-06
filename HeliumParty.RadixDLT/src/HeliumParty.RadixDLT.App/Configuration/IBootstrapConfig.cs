using HeliumParty.RadixDLT.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT.Configuration
{
    public interface IBootstrapConfig
    {
        RadixUniverseConfig Config { get; set; }
        List<IRadixNetworkEpic> DiscoveryEpics { get; set; }
        HashSet<RadixNode> InitialNetwork { get; set; }
    }
}
