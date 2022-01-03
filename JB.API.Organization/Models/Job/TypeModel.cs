using JB.Infrastructure.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace JB.Organization.Models.Job
{
    public class TypeModel : IEntityPrimaryKey
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        [JsonIgnore]
        public virtual ICollection<JobModel> Jobs { get; set; }
    }
}
