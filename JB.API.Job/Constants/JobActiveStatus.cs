using System.ComponentModel;

namespace JB.Job.Constants
{
    public enum JobActiveStatus
    {
        [Description("New")]
        NEW = 0,
        [Description("Hiring")]
        HIRING = 1,
        [Description("Locked")]
        LOCKED = 2,
        [Description("Expired")]
        EXPIRED = 3,
        [Description("Closed")]
        CLOSED = 4,
    }
}
