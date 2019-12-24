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


    }
}
