using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT.Jsonrpc
{
    public class NodeRunnerData // TODO Serialization missing
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

        public override string ToString() => $"{((_IP != null) ? $"{_IP}: " : "")}shards={GetShards().ToString()}";

        public override int GetHashCode() => (_IP + _System.GetShards().ToString()).GetHashCode();  // TODO: Java lib might change here

        public override bool Equals(object obj)
        {
            if (obj != null && obj is NodeRunnerData nrd)
                return nrd._IP.Equals(_IP) && nrd.GetShards().Equals(GetShards());

            return false;
        }
    }
}
