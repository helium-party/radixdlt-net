using HeliumParty.RadixDLT.Atoms;
using System;

namespace HeliumParty.RadixDLT.Actions
{
    public class SubmitAtomCompleteAction : ISubmitAtomAction
    {
        public string Id { get; }
        public Atom AtomSubmission { get; }
        public RadixNode Node { get; }

        public SubmitAtomCompleteAction(string id, Atom atom, RadixNode node)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            AtomSubmission = atom ?? throw new ArgumentNullException(nameof(atom));
            Node = node ?? throw new ArgumentNullException(nameof(node));
        }

        public override string ToString() =>
            $"SUBMIT_ATOM_COMPLETE {Id} {AtomSubmission.Id} {Node}";
    }
}
