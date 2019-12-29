using System;
using System.Linq;
using System.Collections.Generic;
using HeliumParty.RadixDLT.Atoms;

namespace HeliumParty.RadixDLT.Actions
{
    /// <summary>
    /// The initial dispatchable action to begin an atom submission flow
    /// </summary>
    public class SubmitAtomRequestAction : ISubmitAtomAction, IFindANodeRequestAction
    {
        public HashSet<long> Shards => AtomSubmission.GetRequiredFirstShard();
        public RadixNode Node => throw new InvalidOperationException(); // TODO: Another interface for this class!
        public string Id { get; }
        public Atom AtomSubmission { get; }

        public bool IsCompleteOnStoreOnly { get; }

        private SubmitAtomRequestAction(string id, Atom atom, bool completeOnStoreOnly)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            AtomSubmission = atom ?? throw new ArgumentNullException(nameof(atom));
            IsCompleteOnStoreOnly = completeOnStoreOnly;
        }

        public static SubmitAtomRequestAction NewRequest(Atom atom, bool completeOnStoreOnly)
        {
            if (atom.GetRequiredFirstShard().Count == 0)
                throw new ArgumentException($"Atom has no destination {atom}");

            return new SubmitAtomRequestAction(Guid.NewGuid().ToString(), atom, completeOnStoreOnly);
        }

        public override string ToString() =>
            $"SUBMIT_ATOM_REQUEST {Id} {AtomSubmission.Id}";
    }
}
