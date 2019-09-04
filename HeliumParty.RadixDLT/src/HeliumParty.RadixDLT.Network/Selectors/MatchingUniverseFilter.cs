namespace HeliumParty.RadixDLT.Selectors
{
    /// <summary>
    /// A universe filter that checks if peers have the same <see cref="RadixUniverseConfig"/>
    /// </summary>
    public class MatchingUniverseFilter
    {
        /// <summary>
        /// The specified <see cref="RadixUniverseConfig"/>
        /// </summary>
        public RadixUniverseConfig UniverseConfig { get; }

        /// <summary>
        /// Basic constructor to specify the <see cref="RadixUniverseConfig"/>
        /// </summary>
        /// <param name="universeConfig"></param>
        public MatchingUniverseFilter(RadixUniverseConfig universeConfig)
        {
            UniverseConfig = universeConfig ?? throw new System.ArgumentNullException(nameof(universeConfig));
        }

        /// <summary>
        /// Test whether the specified <see cref="RadixNodeState"/> matches the stored <see cref="RadixUniverseConfig"/>
        /// </summary>
        /// <param name="nodeState">The <see cref="RadixNodeState"/> to check</param>
        /// <returns>On true universe configs match</returns>
        public bool Test(RadixNodeState nodeState)
        {
            if (nodeState?.UniverseConfig == null)
                return false;
            return nodeState.UniverseConfig.Equals(UniverseConfig);
        }
    }
}
