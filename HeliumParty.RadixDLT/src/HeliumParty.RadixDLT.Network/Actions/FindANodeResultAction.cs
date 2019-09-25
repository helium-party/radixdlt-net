namespace HeliumParty.RadixDLT.Actions
{
    /// <summary>
    /// The result of a <see cref="IFindANodeRequestAction"/>
    /// </summary>
    public class FindANodeResultAction : IRadixNodeAction
    {
        /// <summary>
        /// The node selected for the request
        /// </summary>
        public RadixNode Node { get; }

        /// <summary>
        /// The original request
        /// </summary>
        public IFindANodeRequestAction Request { get; }

        private FindANodeResultAction(RadixNode node, IFindANodeRequestAction request)
        {
            Node = node ?? throw new System.ArgumentNullException(nameof(node));
            Request = request ?? throw new System.ArgumentNullException(nameof(request));
        }

        public static FindANodeResultAction From(RadixNode node, IFindANodeRequestAction request) => new FindANodeResultAction(node, request);

        public override string ToString() => $"FIND_A_NODE_RESULT {Node} {Request}";
    }
}
