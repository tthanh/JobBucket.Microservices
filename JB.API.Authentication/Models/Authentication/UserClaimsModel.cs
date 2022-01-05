using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JB.Lib.Models.User
{
    public class UserClaimsModel : IUserClaimsModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
    }

    public interface IUserClaimsModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
    }
}