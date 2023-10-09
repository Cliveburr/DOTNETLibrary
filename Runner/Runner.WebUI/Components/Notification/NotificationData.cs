namespace Runner.WebUI.Components.Notification
{
    public class NotificationData
    {
        public required string Text { get; set; }
        public required string Class { get; set; }
        public Timer? Timer { get; set; }
    }
}
