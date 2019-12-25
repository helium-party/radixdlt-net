using HeliumParty.RadixDLT.Atoms;

namespace HeliumParty.RadixDLT.Ledger
{
    public class AtomObservation
    {
        #region Public members

        /// <summary>
        /// Atom that is being observed
        /// </summary>
        public Atom ObservedAtom { get; }

        /// <summary>
        /// Time ticks of operation system at creation of this <see cref="AtomObservation"/> instance
        /// </summary>
        public long TimeStamp { get; }

        public AtomObservationState UpdateState { get; }

        public AtomStorageType StorageType => UpdateState.StorageType;

        public bool HasAtom => UpdateState.StorageType == AtomStorageType.Store
            || UpdateState.StorageType == AtomStorageType.Delete;

        public bool IsStore => UpdateState.StorageType == AtomStorageType.Store;

        public bool IsHead => UpdateState.StorageType == AtomStorageType.Head;

        #endregion
        
        private AtomObservation(Atom atom, AtomStorageType type, bool isSoft)
        {
            ObservedAtom = atom;
            UpdateState = new AtomObservationState(type, isSoft);
            TimeStamp = System.DateTime.UtcNow.Ticks;
        }

        #region Static methods

        /// <summary>
        /// An atom stored observation marked as soft, meaning that it has been confirmed to being stored 
        /// by a server via a submission but is not part of the normal server fetch atom flow and so must 
        /// be handled as "soft state", state which to the clients knowledge is stored but can easily be
        /// replaced by "harder" state.
        /// </summary>
        /// <param name="atom">The atom which is soft stored</param>
        /// <returns>The atom stored observation</returns>
        public static AtomObservation SoftStored(Atom atom) => new AtomObservation(atom, AtomStorageType.Store, true);
        public static AtomObservation SoftDeleted(Atom atom) => new AtomObservation(atom, AtomStorageType.Delete, true);
        public static AtomObservation Stored(Atom atom) => new AtomObservation(atom, AtomStorageType.Store, false);
        public static AtomObservation Deleted(Atom atom) => new AtomObservation(atom, AtomStorageType.Delete, false);
        public static AtomObservation Head() => new AtomObservation(null, AtomStorageType.Head, false);

        public static AtomObservation OfEvent(AtomEvent atomEvent) => new AtomObservation(atomEvent.EventAtom, atomEvent.GetStorageType(), false);

        #endregion

        public override string ToString() => $"{UpdateState.StorageType} {ObservedAtom}";
    }
}
