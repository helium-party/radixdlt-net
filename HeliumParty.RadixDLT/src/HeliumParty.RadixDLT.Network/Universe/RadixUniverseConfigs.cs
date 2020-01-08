using System.Reflection;

namespace HeliumParty.RadixDLT.Universe
{
    public static class RadixUniverseConfigs
    {
        public static RadixUniverseConfig GetLocalnet() => RadixUniverseConfig.FromBytes(Resources.localnet);
        public static RadixUniverseConfig GetBetanet() => RadixUniverseConfig.FromBytes(Resources.betanet);
        public static RadixUniverseConfig GetWinterfell() => RadixUniverseConfig.FromBytes(Resources.testuniverse);
        public static RadixUniverseConfig GetSunstone() => RadixUniverseConfig.FromBytes(Resources.sunstone);
        public static RadixUniverseConfig GetHighgarden() => RadixUniverseConfig.FromBytes(Resources.highgarden);
        public static RadixUniverseConfig GetAlphanet() => RadixUniverseConfig.FromBytes(Resources.alphanet);
    }
}
