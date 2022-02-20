using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace JB.API.User.Models.Job
{
    public class JobModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string[] Cities { get; set; }
        public ICollection<JobSkillModel> Skills { get; set; }
        public ICollection<JobPositionModel> Positions { get; set; }
        public ICollection<JobCategoryModel> Categories { get; set; }
        public ICollection<JobTypeModel> Types { get; set; }
        public int EmployerId { get; set; }
    }
}
