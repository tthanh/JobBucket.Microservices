using System.ComponentModel;

namespace JB.Job.Constants
{
    public enum ApplicationStatus
    {
        [Description("Applied")]
        APPLIED = 1,
        [Description("Cancelled")]
        CANCELLED = 2,
        [Description("In Review")]
        IN_REVIEW = 3,
        [Description("Interview")]
        INTERVIEW = 4,
        [Description("Offer")]
        OFFER = 5,
        [Description("Passed")]
        PASSED = 6,
        [Description("Failed")]
        FAILED = 7,
    }
}
