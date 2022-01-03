using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JB.Infrastructure.Constants
{
    public enum RoleType
    {
        [Description(Role.USER)]
        User = 1,

        [Description(Role.RECRUITER)]
        Recruiter = 2,

        [Description(Role.ORGANIZATION_MANAGER)]
        OrganizationManager = 3,

        [Description(Role.CUSTOMER_CARE)]
        CustomerCare = 4,

        [Description(Role.ADMIN)]
        Admin = 5,
    }

    public static class Role
    {
        public static Dictionary<string, RoleType> FromString = new Dictionary<string, RoleType>
        {
            [USER] = RoleType.User,
            [RECRUITER] = RoleType.Recruiter,
            [ORGANIZATION_MANAGER] = RoleType.OrganizationManager,
            [CUSTOMER_CARE] = RoleType.CustomerCare,
            [ADMIN] = RoleType.Admin,
        };

        public const string USER = "User";
        public const string RECRUITER = "Recruiter";
        public const string ORGANIZATION_MANAGER = "Organization Manager";
        public const string CUSTOMER_CARE = "Customer Care";
        public const string ADMIN = "Administrator";
    }
}
