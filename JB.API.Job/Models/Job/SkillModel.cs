using JB.Infrastructure.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace JB.Job.Models.Job
{
    public class SkillModel : IEntityPrimaryKey
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public virtual ICollection<JobModel> Jobs { get; set; }
    }
}
