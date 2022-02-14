using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Infrastructure.Elasticsearch.Job.Property
{
    public class JobApplicationDocument
    {
        public int UserId { get; set; }
        public int JobId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
