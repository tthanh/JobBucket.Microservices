namespace JB.Notification.DTOs.Notification
{
    public class NotificationRequest
    {
        public int Type { get; set; }
        public int Level { get; set; }
        public int? SenderId { get; set; }
        public int? OrganizationId { get; set; }
        public int ReceiverId { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }
    }
}
