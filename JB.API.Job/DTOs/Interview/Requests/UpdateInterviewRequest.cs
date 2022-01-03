using JB.Job.Models.Interview;
using System;

namespace JB.Job.DTOs.Interview
{
    public class UpdateInterviewRequest
    {
        public int Id { get; set; }
        public DateTime InterviewTime { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public int JobId { get; set; }
      
        public int IntervieweeId { get; set; }
     
        public int IntervieweeCVId { get; set; }
     
        public int InterviewerId { get; set; }
        
       
        public InterviewFormModel Form { get; set; }
    }
}
