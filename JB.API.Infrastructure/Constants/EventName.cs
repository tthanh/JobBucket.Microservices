using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JB.Infrastructure.Constants
{
    public enum EventName
    {
        [Description("job.added")]
        JOB_ADDED,
        [Description("job.updated")]
        JOB_UPDATED,
        [Description("job.deleted")]
        JOB_DELETED,

        [Description("blog.added")]
        BLOG_ADDED,
        [Description("blog.updated")]
        BLOG_UPDATED,
        [Description("blog.deleted")]
        BLOG_DELETED,

        [Description("profile.added")]
        PROFILE_ADDED,
        [Description("profile.updated")]
        PROFILE_UPDATED,
    }
}
