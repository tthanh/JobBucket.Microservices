namespace JB.Notification.DTOs.User
{
    public class NotificationUserResponse
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string AvatarUrl { get; set; }

        public int OrganizationId { get; set; }
    }
}
