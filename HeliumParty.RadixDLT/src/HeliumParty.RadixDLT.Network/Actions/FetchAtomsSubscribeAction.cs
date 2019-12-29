using System.Collections.Generic;
using HeliumParty.RadixDLT.Identity;

namespace HeliumParty.RadixDLT.Actions
{
    /// <summary>
    /// A dispatchable fetch atoms action which represents a fetch atom query submitted to a specific node.
    /// </summary>
    public class FetchAtomsSubscribeAction : IFetchAtomsAction
    {
        public string Id { get; }
        public RadixAddress Address { get; }
        public RadixNode Node { get; }
                
        public FetchAtomsSubscribeAction(string id, RadixAddress address, RadixNode node)
        {
            Id = id ?? throw new System.ArgumentNullException(nameof(id));
            Address = address ?? throw new System.ArgumentNullException(nameof(address));
            Node = node ?? throw new System.ArgumentNullException(nameof(node));
        }

        public override string ToString() => $"FETCH_ATOMS_SUBSCRIBE {Id} {Address} {Node}";
    }
}
