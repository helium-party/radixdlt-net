namespace HeliumParty.RadixDLT.Atoms
{
    public enum AtomStatus
    {
        DoesNotExist,
        EvictedInvalidAtom,
        EvictedFailedCmVerification,
        EvictedConflictLoser,
        PendingCmVerification,
        PendingDependencyVerification,
        MissingDependency,
        ConflictLoser,
        Stored
    }
}