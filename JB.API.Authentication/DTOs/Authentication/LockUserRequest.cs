using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Authentication.DTOs.Authentication
{
    public class LockUserRequest
    {
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime LockUntil { get; set; }
    }
}
