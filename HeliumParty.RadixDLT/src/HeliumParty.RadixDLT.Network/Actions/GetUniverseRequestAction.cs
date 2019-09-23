namespace HeliumParty.RadixDLT.Actions
{
    public class GetUniverseRequestAction : IRadixNodeAction
    {
        public RadixNode Node { get; }

        private GetUniverseRequestAction(RadixNode node)
        {
            Node = node ?? throw new System.ArgumentNullException(nameof(node));
        }

        public static GetUniverseRequestAction From(RadixNode node) => new GetUniverseRequestAction(node);

        public override string ToString() => "GET_UNIVERSE_REQUEST " + Node;
    }
}
