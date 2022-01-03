namespace JB.Job.DTOs.Job
{
    public class InterestResponse
    {
        public JobUserResponse User { get; set; }
        public int UserId { get; set; }
        public JobResponse Job { get; set; }
        public int JobId { get; set; }
    }
}
