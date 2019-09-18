namespace HeliumParty.RadixDLT.Log
{
    public class OutputLogger : ILogger
    {
        public LogLevel LogLevel { get; set; }
        public bool IsDebugEnabled { get; set; }

        public const int MaxMessageSize = 1024;

        public void LogMessage(string message, LogLevel level = LogLevel.Verbose)
        {
            System.Diagnostics.Debug.Print($"{BuildTimeStamp()} - {message}");
        }

        private string BuildTimeStamp()
        {
            var dtn = System.DateTime.Now;
            return $"{ dtn.ToShortDateString()} - { dtn.ToLongTimeString()} {dtn.Millisecond.ToString("D3")}";
        }

        /// <summary>
        /// Ensures that the message doesn't exceed the specified max length
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private string EnsureMessageLength(string message)
        {
            if (message.Length > MaxMessageSize)
                return message.Substring(0, MaxMessageSize - 3 /*for the '...'*/) + "...";
            else
                return message;
        }
    }
}
