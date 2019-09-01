using System.Collections.Generic;
using HeliumParty.RadixDLT.Identity;

namespace HeliumParty.RadixDLT.Actions
{
    /// <summary>
    /// A dispatchable fetch atoms action which represents a fetch atom query submitted to a specific node.
    /// </summary>
    public class FetchAtomsSubscribeAction : IFetchAtomsAction
    {
        public string UUID { get; }
        public RadixAddress Address { get; }
        public RadixNode Node { get; }
                
        public FetchAtomsSubscribeAction(string uuid, RadixAddress address, RadixNode node)
        {
            UUID = uuid ?? throw new System.ArgumentNullException(nameof(uuid));
            Address = address ?? throw new System.ArgumentNullException(nameof(address));
            Node = node ?? throw new System.ArgumentNullException(nameof(node));
        }

        public override string ToString() => $"FETCH_ATOMS_SUBSCRIBE {UUID} {Address} {Node}";
    }
}
