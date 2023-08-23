using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Service.Interface
{
    public interface IEmailSerivce
    {
        Task SendEmailAsync(Dictionary<string, string> data);
    }
}
