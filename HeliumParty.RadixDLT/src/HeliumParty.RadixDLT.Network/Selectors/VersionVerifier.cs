namespace HeliumParty.RadixDLT.Selectors
{
    public class VersionVerifier
    {
        /// <summary>
        /// The version of the verifier
        /// </summary>
        public int Version { get; }
        public VersionVerifier(int version)
        {
            Version = version;
        }

        /// <summary>
        /// Verifies if the specified version may be used
        /// </summary>
        /// <param name="nodeState">This nodes version should be verified</param>
        /// <returns>Whether version is valid (= true)</returns>
        public bool Verify(RadixNodeState nodeState)
        {
            if (nodeState?.Version != null)
                return nodeState.Version == Version;

            return false;
        }
    }
}
