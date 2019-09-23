namespace HeliumParty.RadixDLT.Actions
{
    /// <summary>
    /// An action which represents a node was found which doesn't match the universe expected
    /// </summary>
    public class NodeUniverseMismatchAction : IRadixNodeAction
    {
        public RadixNode Node { get; }
        public Universe.RadixUniverseConfig ExpectedConfig { get; }
        public Universe.RadixUniverseConfig ActualConfig { get; }

        public NodeUniverseMismatchAction(RadixNode node, Universe.RadixUniverseConfig expected, Universe.RadixUniverseConfig actual)
        {
            Node = node;
            ExpectedConfig = expected;
            ActualConfig = actual;
        }

        public override string ToString() => $"NODE_UNIVERSE_MISMATCH {Node} expected: {ExpectedConfig} but was {ActualConfig}";
    }
}
