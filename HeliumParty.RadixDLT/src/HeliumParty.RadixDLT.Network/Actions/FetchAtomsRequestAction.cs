using System.Collections.Generic;
using HeliumParty.RadixDLT.Identity;

namespace HeliumParty.RadixDLT.Actions
{
    /// <summary>
    /// The initial dispatchable fetch atoms action which signals a new fetch atoms query request
    /// </summary>
    public class FetchAtomsRequestAction : IFetchAtomsAction, IFindANodeRequestAction
    {
        public string Id { get; }
        public RadixAddress Address { get; }
        public RadixNode Node => throw new System.InvalidOperationException("This action doesn't contain a node!");
        public HashSet<long> Shards => new HashSet<long>( new long[] { Address.GetId().Shard });
        
        private FetchAtomsRequestAction(string id, RadixAddress address)
        {
            Id = id ?? throw new System.ArgumentNullException(nameof(id));
            Address = address ?? throw new System.ArgumentNullException(nameof(address));
        }

        public FetchAtomsRequestAction(RadixAddress address) : this(System.Guid.NewGuid().ToString(), address)
        { }

        public override string ToString() => $"FETCH_ATOMS_REQUEST {Id} {Address}";
    }
}
