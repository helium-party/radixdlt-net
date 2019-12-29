namespace HeliumParty.RadixDLT.Jsonrpc
{
    public class JsonRpcNotification<T>
    {
        public T NotificationEvent { get; }
        public JsonRpcNotificationType NotificationType { get; }

        public JsonRpcNotification(JsonRpcNotificationType type, T notificationEvent)
        {
            NotificationType = type;
            NotificationEvent = notificationEvent;
        }

        public static JsonRpcNotification<T> OfEvent(T notificationEvent) 
            => new JsonRpcNotification<T>(JsonRpcNotificationType.Event, notificationEvent);
    }
}
