using System;

namespace JB.Job.DTOs.Interview
{
    public class AddInterviewRequest
    {
        public DateTime InterviewTime { get; set; }
        public string Description { get; set; }
        public int JobId { get; set; }
        public int IntervieweeId { get; set; }
        public int InterviewerId { get; set; }
        public int? IntervieweeCVId { get; set; }
        public int TotalInterviewRound { get; set; }
    }
}
