namespace JB.Notification.DTOs.Organization
{
    public class OrganizationResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Bio { get; set; }

        public string Country { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string[] Addresses { get; set; }

        public string[] ImageUrls { get; set; }

        public string AvatarUrl { get; set; }
    }
}