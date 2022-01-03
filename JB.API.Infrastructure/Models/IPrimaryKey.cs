using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Infrastructure.Models
{
    public interface IEntityPrimaryKey
    {
        int Id { get; set; }
    }
}
