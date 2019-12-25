namespace HeliumParty.RadixDLT.Ledger
{
    /// <summary>
    /// Wrapper reducer class for <see cref="InMemoryAtomStore"/>
    /// </summary>
    public class InMemoryAtomStoreReducer
    {
        private readonly InMemoryAtomStore _AtomStore;
        public InMemoryAtomStoreReducer(InMemoryAtomStore atomStore)
        {
            _AtomStore = atomStore ?? throw new System.ArgumentNullException(nameof(atomStore));
        }

        public void Reduce(IRadixNodeAction action)
        {
            if (action is Actions.FetchAtomsObservationAction fetchAction)
                _AtomStore.Store(fetchAction.Address, fetchAction.Observation);

            else if (action is Actions.SubmitAtomStatusAction submitAction)
            {
                // Soft storage of atoms so that atoms which are submitted and stored can be immediately
                // used instead of having to wait for fetch atom events
                if (submitAction.StatusNotification.Status == Atoms.AtomStatus.Stored)
                {
                    foreach (var addr in submitAction.AtomSubmission.GetAllAddresses())
                    {
                        _AtomStore.Store(addr, AtomObservation.SoftStored(submitAction.AtomSubmission));
                        _AtomStore.Store(addr, AtomObservation.Head());
                    }
                }
            }
        }
    }
}
