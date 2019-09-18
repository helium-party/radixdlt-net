namespace HeliumParty.RadixDLT.Log
{
    public interface ILogger
    {
        bool IsDebugEnabled { get; set; }

        /// <summary>
        /// Defines the minimum level of messages to log
        /// </summary>
        LogLevel LogLevel { get; set; }

        /// <summary>
        /// Logs the specified message
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="level">The log level of the message</param>
        void LogMessage(string message, LogLevel level = LogLevel.Verbose);
    }
}
