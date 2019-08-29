using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT
{
    /// <summary>
    /// An action utilized in the Radix Network epics and reducers.
    /// </summary>
    interface IRadixNodeAction
    {
        /// <summary>
        /// The node associated with the network action
        /// </summary>
        /// <returns>The associated radix node</returns>
        RadixNode GetNode();
    }
}
