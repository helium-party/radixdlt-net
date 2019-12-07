namespace HeliumParty.RadixDLT.Jsonrpc
{
    public class JsonRpcNotification<T>
    {
        public T NotificationEvent { get; }
        public JsonRpcNotificationType NotificationType { get; }    // TODO: Don't think we really need the type..

        public JsonRpcNotification(JsonRpcNotificationType type, T notificationEvent)
        {
            NotificationType = type;
            NotificationEvent = notificationEvent;
        }
    }
}
