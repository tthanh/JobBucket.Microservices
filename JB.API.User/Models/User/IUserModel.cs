using JB.Infrastructure.Models;
using JB.User.Models.Organization;

namespace JB.User.Models.User
{
    public interface IUserModel : IEntityDate
    {
        public string Name { get; set; }

        public int RoleId { get; set; }

        public string AvatarUrl { get; set; }

        public int? OrganizationId { get; set; }

        public OrganizationModel Organization { get; set; }

        public int? DefaultCVId { get; set; }

        public string PhoneNumber { get; set; }

        public bool EmailConfirmed { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public int Id { get; set; }
    }
}
