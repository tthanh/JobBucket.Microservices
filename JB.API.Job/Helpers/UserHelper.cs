﻿using JB.Job.Models.Organization;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace JB.Job.Helpers
{
    public static class UserHelper
    {
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
