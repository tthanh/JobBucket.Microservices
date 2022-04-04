using JB.Infrastructure.Models;
using JB.Job.Models.CV;
using JB.Job.Models.Job;
using JB.Job.Models.Organization;
using JB.Job.Models.User;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace JB.Job.Models.Interview
{
    public class InterviewModel : IEntityDate
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime InterviewTime { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public int JobId { get; set; }
        [NotMapped]
        public JobModel Job { get; set; }
        public int IntervieweeId { get; set; }
        [NotMapped]
        public UserModel Interviewee { get; set; }
        public int IntervieweeCVId { get; set; }
        [NotMapped]
        public CVModel IntervieweeCV { get; set; }
        public int InterviewerId { get; set; }
        [NotMapped]
        public UserModel Interviewer { get; set; }
        public int OrganizationId { get; set; }
        [NotMapped]
        public OrganizationModel Organization { get; set; }
        [Column(TypeName = "jsonb")]
        public ICollection<InterviewFormModel> Forms { get; set; }
        public int TotalInterviewRound { get; set; }
        public int CurrentInterviewRound { get; set; }
    }
}
