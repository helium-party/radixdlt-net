using HeliumParty.RadixDLT.Identity;

namespace HeliumParty.RadixDLT.Actions
{
    /// <summary>
    /// A dispatchable fetch atoms action which represents an atom observed event 
    /// from a specific node for an atom fetch flow.
    /// </summary>
    public class FetchAtomsOversavtionAction : IFetchAtomsAction
    {
        public string UUID { get; }
        public RadixAddress Address { get; }
        public RadixNode Node { get; }

        /// <summary>
        /// The atom observation associated with this action
        /// </summary>
        public AtomObservation Observation { get; }

        public FetchAtomsOversavtionAction(
            string uuid, 
            RadixAddress address, 
            RadixNode node, 
            AtomObservation observation)
        {
            UUID = uuid ?? throw new System.ArgumentNullException(nameof(uuid));
            Address = address ?? throw new System.ArgumentNullException(nameof(address));
            Node = node ?? throw new System.ArgumentNullException(nameof(node));
            Observation = observation ?? throw new System.ArgumentNullException(nameof(observation));
        }

        public override string ToString() => $"FETCH_ATOMS_OBSERVATION {Node} {UUID} {Observation}";
    }
}
