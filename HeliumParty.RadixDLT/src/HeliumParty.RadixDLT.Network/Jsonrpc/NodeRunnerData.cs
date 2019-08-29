using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT.Jsonrpc
{
    public class NodeRunnerData : Radix.Serialization.Client.SerializableObject
    {
        private string _IP;

        private RadixSystem _System;

        protected NodeRunnerData(RadixSystem system)
        {
            _IP = null;
            _System = system;
        }

        public ShardSpace GetShards() => _System.GetShards();

        public string GetIP() => _IP;

        // TODO: Continue implementation

    }
}
