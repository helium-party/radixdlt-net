using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT.Jsonrpc
{
    /// <summary>
    /// Simple API version used for syncing between server/client
    /// </summary>
    public class ApiVersion
    {
        /// <summary>
        /// Version of the API
        /// </summary>
        public int Version { get; }

        // TODO: When is this method called!? Seems to be a code-corpse...
        private ApiVersion(int version)
        {
            Version = version;
        }
    }
}
