using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Ledger;

namespace HeliumParty.RadixDLT.Actions
{
    /// <summary>
    /// A dispatchable fetch atoms action which represents an atom observed event 
    /// from a specific node for an atom fetch flow.
    /// </summary>
    public class FetchAtomsObservationAction : IFetchAtomsAction
    {
        public string Id { get; }
        public RadixAddress Address { get; }
        public RadixNode Node { get; }

        /// <summary>
        /// The atom observation associated with this action
        /// </summary>
        public AtomObservation Observation { get; }

        public FetchAtomsObservationAction(
            string id, 
            RadixAddress address, 
            RadixNode node, 
            AtomObservation observation)
        {
            Id = id ?? throw new System.ArgumentNullException(nameof(id));
            Address = address ?? throw new System.ArgumentNullException(nameof(address));
            Node = node ?? throw new System.ArgumentNullException(nameof(node));
            Observation = observation ?? throw new System.ArgumentNullException(nameof(observation));
        }

        public override string ToString() => $"FETCH_ATOMS_OBSERVATION {Node} {Id} {Observation}";
    }
}
