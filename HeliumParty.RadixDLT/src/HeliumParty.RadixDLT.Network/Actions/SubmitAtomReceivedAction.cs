using HeliumParty.RadixDLT.Atoms;
using System;

namespace HeliumParty.RadixDLT.Actions
{
    /// <summary>
    /// A dispatchable action which signifies that an atom being submitted was received by the node
    /// </summary>
    public class SubmitAtomReceivedAction : ISubmitAtomAction
    {
        public string Id { get; }
        public Atom AtomSubmission { get; }
        public RadixNode Node { get; }

        public SubmitAtomReceivedAction(string id, Atom atom, RadixNode node)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            AtomSubmission = atom ?? throw new ArgumentNullException(nameof(atom));
            Node = node ?? throw new ArgumentNullException(nameof(node));
        }

        public override string ToString() =>
            $"SUBMIT_ATOM_RECEIVED {Id} {AtomSubmission.Id} {Node}";
    }
}
