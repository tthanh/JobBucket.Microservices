using JB.Job.Models.Interview;
using System;
using System.Collections.Generic;

namespace JB.Job.DTOs.Interview
{
    public class UpdateInterviewRequest
    {
        public int Id { get; set; }
        public DateTime InterviewTime { get; set; }
        public string Description { get; set; }
        public int JobId { get; set; }
  

        public ICollection<InterviewFormModel> Forms { get; set; }
    }
}
