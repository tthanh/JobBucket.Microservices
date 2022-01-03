using JB.Job.DTOs.Job.Property;
using System.Collections.Generic;

namespace JB.Job.DTOs.Job
{
    public class JobPropertiesResponse
    {
        public ICollection<JobSkillResponse> Skills { get; set; }
        
        public ICollection<JobPositionResponse> Positions { get; set; }
        
        public ICollection<JobTypeResponse> Types { get; set; }
        
        public ICollection<JobCategoryResponse> Categories { get; set; }
    }
}
