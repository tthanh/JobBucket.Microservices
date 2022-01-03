using JB.User.Models.Organization;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace JB.User.Helpers
{
    public static class UserHelper
    {
        public static string GenRandomPassword(int length)
        {
            byte[] rgb = new byte[length];
            RNGCryptoServiceProvider rngCrypt = new RNGCryptoServiceProvider();
            rngCrypt.GetBytes(rgb);
            return Convert.ToBase64String(rgb);
        }

        public static bool IsRecruiter(int recruiterId, OrganizationModel organization)
        {
            if (recruiterId <= 0
                || organization == null
                || organization.EmployerIds == null
                || !organization.EmployerIds.Contains(recruiterId))
            {
                return false;
            }
            return true;
        }

        public static bool IsManager(int managerId, OrganizationModel organization)
        {
            if (managerId <= 0
                || organization == null
                || organization.ManagerIds == null
                || !organization.ManagerIds.Contains(managerId))
            {
                return false;
            }
            return true;
        }
    }
}
