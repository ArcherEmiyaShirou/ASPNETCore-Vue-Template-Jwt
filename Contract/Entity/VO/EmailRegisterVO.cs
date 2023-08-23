using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Backend.Contract.Entity.VO
{
    public class EmailRegisterVO
    {
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(maximumLength: 6, MinimumLength = 6)]
        public string? Code { get; set; }

        [RegularExpression(pattern:"^[a-zA-Z0-9\\u4e00-\\u9fa5]+$")]
        [StringLength(10,MinimumLength = 1)]
        public string? Username { get; set; }

        [StringLength(20,MinimumLength = 6)]
        public string? Password { get; set; }
    }
}
