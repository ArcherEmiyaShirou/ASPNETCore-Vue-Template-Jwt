using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Contract.Entity.VO
{
    public class EmailResetVO
    {
        [EmailAddress]
        public string? email { get; set; }
        [StringLength(6,MinimumLength = 6)]
        public string? code { get; set; }
        [StringLength(20,MinimumLength = 6)]
        public string? password { get; set; }
    }
}
