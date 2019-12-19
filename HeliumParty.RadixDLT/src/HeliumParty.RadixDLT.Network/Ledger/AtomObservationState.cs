namespace HeliumParty.RadixDLT.Ledger
{
    // TODO: Is name fitting?
    /// <summary>
    /// Describes the type of observation including whetehr the update is "soft", 
    /// or a weakly supported atom which could possibly be deleted soon.
    /// </summary>
    public class AtomObservationState
    {
        public AtomStorageType StorageType { get; }
        public bool IsSoft { get; }

        public AtomObservationState(AtomStorageType type, bool isSoft)
        {
            StorageType = type;
            IsSoft = isSoft;
        }
    }
}
