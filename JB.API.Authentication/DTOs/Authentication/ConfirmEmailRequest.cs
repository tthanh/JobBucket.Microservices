﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Authentication.DTOs.Authentication
{
    public class ConfirmEmailRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Code { get; set; }
    }
}
