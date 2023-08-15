using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Contract.Entity;

namespace Backend.Service.Interface
{
    public interface IAccountService
    {
        AuthorizeVO Login();
    }
}
