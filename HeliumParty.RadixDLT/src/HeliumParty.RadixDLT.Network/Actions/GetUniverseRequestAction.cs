namespace HeliumParty.RadixDLT.Actions
{
    public class GetUniverseRequestAction : IJsonRpcMethodAction
    {
        public RadixNode Node { get; }

        public GetUniverseRequestAction(RadixNode node)
        {
            Node = node ?? throw new System.ArgumentNullException(nameof(node));
        }

        public override string ToString() => "GET_UNIVERSE_REQUEST " + Node;
    }
}
