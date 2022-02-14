using JB.Job.Models.Interview;
using System;

namespace JB.Job.DTOs.Interview
{
    public class InterviewResponse
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime InterviewTime { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public int JobId { get; set; }
        
        public InterviewJobResponse Job { get; set; }
        public int IntervieweeId { get; set; }
      
        public InterviewUserResponse Interviewee { get; set; }
        public int IntervieweeCVId { get; set; }
      
        public InterviewCVResponse IntervieweeCV { get; set; }
        public int InterviewerId { get; set; }
      
        public InterviewUserResponse Interviewer { get; set; }
        public int OrganizationId { get; set; }
      
        public InterviewOrganizationResponse Organization { get; set; }
     
        public InterviewFormModel Form { get; set; }
    }
}
