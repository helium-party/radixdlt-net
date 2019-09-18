namespace HeliumParty.RadixDLT.Web
{
    public class WebSocketException : System.Exception
    {
        public WebSocketException() : base() { }
        public WebSocketException(string message) : base(message) { }
    }
}
