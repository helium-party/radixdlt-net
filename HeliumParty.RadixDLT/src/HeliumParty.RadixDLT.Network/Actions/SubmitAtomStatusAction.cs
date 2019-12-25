using HeliumParty.RadixDLT.Atoms;
using System;

namespace HeliumParty.RadixDLT.Actions
{
    public class SubmitAtomStatusAction : ISubmitAtomAction
    {
        public string Uuid { get; }
        public Atom AtomSubmission { get; }
        public RadixNode Node { get; }
        
        public AtomStatusEvent StatusNotification { get; }

        public SubmitAtomStatusAction(string uuid, Atom atom, RadixNode node, AtomStatusEvent statusNotification)
        {
            Uuid = uuid ?? throw new ArgumentNullException(nameof(uuid));
            AtomSubmission = atom ?? throw new ArgumentNullException(nameof(atom));
            Node = node ?? throw new ArgumentNullException(nameof(node));
            StatusNotification = statusNotification ?? throw new ArgumentNullException(nameof(statusNotification));
        }

        public override string ToString() =>
            $"SUBMIT_ATOM_STATUS {Uuid} {AtomSubmission.Id} {Node} {StatusNotification}";
    }
}
