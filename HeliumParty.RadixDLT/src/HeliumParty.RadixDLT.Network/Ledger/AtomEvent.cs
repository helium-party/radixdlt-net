using HeliumParty.RadixDLT.Atoms;

namespace HeliumParty.RadixDLT.Ledger
{
    /// <summary>
    /// Serialisation functionality for the <see cref="AtomObservation"/>
    /// </summary>
    public class AtomEvent
    {
        // TODO: Add serialisation!

        // Ignoring the naming convention as this is required to be all lower case
        public enum AtomEventType { store, delete }

        public Atom EventAtom { get; }
        public AtomEventType EventType { get; }

        public AtomEvent(Atom atom, AtomEventType type)
        {
            EventAtom = atom;
            EventType = type;
        }

        public AtomStorageType GetStorageType()
        {
            switch (EventType)
            {
                case AtomEventType.delete:
                    return AtomStorageType.Delete;

                case AtomEventType.store:
                    return AtomStorageType.Store;

                default:
                    throw new System.SystemException($"'{EventType}' is not a valid type");
            }
        }
    }
}
