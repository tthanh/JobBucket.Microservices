namespace JB.Infrastructure.Elasticsearch.User
{
    public class UserDocument
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string AvatarUrl { get; set; }

        public int OrganizationId { get; set; }

        public int ProfileStatus { get; set; }
    }
}
