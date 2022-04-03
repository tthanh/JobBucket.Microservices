using System.ComponentModel;

namespace JB.Job.Constants
{
    public enum ApplicationStatus
    {
        [Description("Applied")]
        APPLIED = 0, // vừa apply
        [Description("Cancelled")]
        CANCELLED = 1,   // ứng viên cancel
        [Description("Processing")]
        PROCESSING = 2, // đang trong quá trình phỏng vấn Interview
        [Description("Passed")]
        PASSED = 3, // đã đậu 
        [Description("Failed")]
        FAILED = 4, // rớt 
    }
}
