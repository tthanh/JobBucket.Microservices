namespace JB.Infrastructure.Messages
{
    public class PromoteUserMessage
    {
        public int UserId { get; set; }
        public int OrganizationId { get; set; }
        public int RoleId { get; set; }
    }
}
