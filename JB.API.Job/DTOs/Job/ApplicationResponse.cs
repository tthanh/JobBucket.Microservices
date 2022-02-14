using JB.Infrastructure.Helpers;
using JB.Job.Constants;
using System;

namespace JB.Job.DTOs.Job
{
    public class ApplicationResponse
    {
        public JobUserResponse User { get; set; }
        public int UserId { get; set; }
        public JobResponse Job { get; set; }
        public int JobId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int Status { get; set; }
        public string StatusDisplay { get => EnumHelper.GetDescriptionFromEnumValue((ApplicationStatus)Status); }
        public int CVId { get; set; }
        public string CVPDFUrl { get; set; }
    }
}
