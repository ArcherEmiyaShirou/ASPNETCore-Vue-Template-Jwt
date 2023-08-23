﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Contract.Entity.VO
{
    public class ConfirmResetVO
    {
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [StringLength(6, MinimumLength = 6)]
        public string Code { get; set; } = string.Empty;
    }
}
