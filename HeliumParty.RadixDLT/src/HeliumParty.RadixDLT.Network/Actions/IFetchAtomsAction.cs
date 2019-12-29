using HeliumParty.RadixDLT.Identity;

namespace HeliumParty.RadixDLT.Actions
{
    /// <summary>
    /// A dispatchable fetch atoms action which represents an atom observed from a 
    /// specific node for an atom fetch flow.
    /// </summary>
    public interface IFetchAtomsAction : IRadixNodeAction
    {
        /// <summary>
        /// The unique ID representing a fetch atoms flow. That is, each type of 
        /// action in a single flow instance must have the same unique id.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The address on which to query atoms from
        /// </summary>
        RadixAddress Address { get; }
    }
}
