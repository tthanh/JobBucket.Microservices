using JB.Notification.Models.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Notification.Models.CV
{
    public class UserEducationModel
    {
        public string School { get; set; }
        public string Major { get; set; }
        public string Status { get; set; }
        public string Profession { get; set; }
    }
}
