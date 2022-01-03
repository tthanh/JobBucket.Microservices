using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JB.Infrastructure.Models.Elasticsearch.User.Property
{
    public class UserEducationDocument
    {
        public string School { get; set; }
        public string Major { get; set; }
        public string Status { get; set; }
        public string Profession { get; set; }
    }
}
