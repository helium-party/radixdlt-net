using HeliumParty.RadixDLT.Atoms;

namespace HeliumParty.RadixDLT.Actions
{
    /// <summary>
    /// A dispatchable fetch atoms action which represents an atom observed 
    /// from a specific node for an atom fetch flow.
    /// </summary>
    public interface ISubmitAtomAction : IRadixNodeAction
    {
        /// <summary>
        /// The unique id representing a fetch atoms flow. That is, each type of 
        /// action in a single flow instance must have the same unique id.
        /// </summary>
        string Uuid { get; }

        /// <summary>
        /// The atom to submit
        /// </summary>
        Atom AtomSubmission { get; }
    }
}
