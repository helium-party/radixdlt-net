namespace HeliumParty.RadixDLT.Jsonrpc
{
    public class ShardRange : Range<long>
    {
        // TODO : Serialization missing...
        public ShardRange(long low, long high) : base(low, high) { }
    }
}
