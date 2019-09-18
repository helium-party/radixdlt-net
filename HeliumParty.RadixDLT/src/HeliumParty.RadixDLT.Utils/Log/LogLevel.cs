namespace HeliumParty.RadixDLT.Log
{
    public enum LogLevel
    {
        // Additional information
        Verbose = 0,

        // Useful for debugging
        Debug = 1,

        // Critical log information
        Critical = 2,
        
        // Don't log any messages
        None = 4,

        // General information
        Information = 99
    }
}
