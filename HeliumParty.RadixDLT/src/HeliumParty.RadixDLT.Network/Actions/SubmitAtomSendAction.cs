using HeliumParty.RadixDLT.Atoms;
using System;

namespace HeliumParty.RadixDLT.Actions
{
    /// <summary>
    /// A dispatchable action which signifies to send an atom to a node
    /// </summary>
    public class SubmitAtomSendAction : ISubmitAtomAction
    {
        public string Id { get; }
        public Atom AtomSubmission { get; }
        public RadixNode Node { get; }

        public bool IsCompleteOnStoreOnly { get; }

        public SubmitAtomSendAction(string id, Atom atom, RadixNode node, bool completeOnStoreOnly)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            AtomSubmission = atom ?? throw new ArgumentNullException(nameof(atom));
            Node = node ?? throw new ArgumentNullException(nameof(node));
            IsCompleteOnStoreOnly = completeOnStoreOnly;
        }
        
        public override string ToString() =>
            $"SUBMIT_ATOM_SEND {Id} {AtomSubmission.Id} {Node}";
    }
}
