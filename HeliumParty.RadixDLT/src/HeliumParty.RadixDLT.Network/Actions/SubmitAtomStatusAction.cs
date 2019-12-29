using HeliumParty.RadixDLT.Atoms;
using System;

namespace HeliumParty.RadixDLT.Actions
{
    /// <summary>
    /// A dispatchable event action which signifies the end result of an atom submission flow
    /// </summary>
    public class SubmitAtomStatusAction : ISubmitAtomAction
    {
        public string Id { get; }
        public Atom AtomSubmission { get; }
        public RadixNode Node { get; }
        
        /// <summary>
        /// The end result type of the atom submission
        /// </summary>
        public AtomStatusEvent StatusNotification { get; }

        public SubmitAtomStatusAction(string id, Atom atom, RadixNode node, AtomStatusEvent statusNotification)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            AtomSubmission = atom ?? throw new ArgumentNullException(nameof(atom));
            Node = node ?? throw new ArgumentNullException(nameof(node));
            StatusNotification = statusNotification ?? throw new ArgumentNullException(nameof(statusNotification));
        }

        public override string ToString() =>
            $"SUBMIT_ATOM_STATUS {Id} {AtomSubmission.Id} {Node} {StatusNotification}";
    }
}
