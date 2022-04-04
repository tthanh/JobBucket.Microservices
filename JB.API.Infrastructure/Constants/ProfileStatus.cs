using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JB.Infrastructure.Constants
{
    public enum ProfileStatus
    {
        [Description("Open to work")]
        OpenToWork = 0,
        [Description("Hidden")]
        Hidden = 1,
    }
}
