using HeliumParty.RadixDLT.Universe;

namespace HeliumParty.RadixDLT.Actions
{
    public class GetUniverseResponseAction : IJsonRpcResultAction<RadixUniverseConfig>
    {
        public RadixNode Node { get; }
        private readonly RadixUniverseConfig _Config;

        private GetUniverseResponseAction(RadixNode node, RadixUniverseConfig config)
        {
            Node = node ?? throw new System.NotImplementedException(nameof(node));
            _Config = config ?? throw new System.NotImplementedException(nameof(config));
        }

        public static GetUniverseResponseAction From(RadixNode node, RadixUniverseConfig config)
        {
            return new GetUniverseResponseAction(node, config);
        }

        public RadixUniverseConfig GetResult() => _Config;

        public override string ToString() => $"GET_UNIVERSE_RESPONSE {Node} {_Config}";
    }
}
