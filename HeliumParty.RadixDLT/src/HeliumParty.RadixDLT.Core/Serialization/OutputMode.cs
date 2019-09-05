using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT.Serialization
{
    public enum OutputMode
    {
        ///// <summary>
        ///// An output mode that never results in output.  Of limited use.
        ///// </summary>
        //None,

        ///// <summary>
        ///// An output mode for calculating hashes.
        ///// </summary>
        //Hash,

        ///// <summary>
        ///// An output mode for use with application interfaces.
        ///// </summary>
        //Api,

        ///// <summary>
        ///// An output mode for use when communicating to other nodes.
        ///// </summary>
        //Wire,

        ///// <summary>
        ///// An output mode for use when writing data to persistent storage.
        ///// </summary>
        //Persist,

        ///// <summary>
        ///// An output mode that always results in output.
        ///// </summary>
        //All,

        /// <summary>
        ///     This should never be serialized
        /// </summary>
        Hidden,
        /// <summary>
        ///     With this mode, only serialize those that have the same mode.
        /// </summary>
        Hash,
        /// <summary>
        ///     Serialize everything that has no mode or a All Mode , but no hidden modes
        /// </summary>
        All,
    }
}
