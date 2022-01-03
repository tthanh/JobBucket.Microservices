using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Organization.DTOs.Organization
{
    public class AddEmployerRequest
    {
        public string Name { get; set; }

        public string UserName { get; set; }

        public string PasswordPlain { get; set; }
    }
}
