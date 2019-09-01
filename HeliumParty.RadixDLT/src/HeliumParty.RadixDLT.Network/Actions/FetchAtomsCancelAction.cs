using HeliumParty.RadixDLT.Identity;

namespace HeliumParty.RadixDLT.Actions
{
    /// <summary>
    /// A dispatchable fetch atoms action which represents a request to cancel a fetch atoms flow
    /// </summary>
    public class FetchAtomsCancelAction : IFetchAtomsAction
    {
        public string UUID { get; }
        public RadixAddress Address { get; }
        public RadixNode Node => throw new System.InvalidOperationException("This action doesn't contain a node!");

        public FetchAtomsCancelAction(string uuid, RadixAddress address)
        {
            UUID = uuid ?? throw new System.ArgumentNullException(nameof(uuid));
            Address = address ?? throw new System.ArgumentNullException(nameof(address));
        }

        public override string ToString() => $"FETCH_ATOMS_CANCEL {UUID} {Address}";
    }
}
