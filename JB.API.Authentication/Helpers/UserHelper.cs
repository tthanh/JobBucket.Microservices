using JB.Authentication.Models.Organization;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace JB.Authentication.Helpers
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
    }
}
