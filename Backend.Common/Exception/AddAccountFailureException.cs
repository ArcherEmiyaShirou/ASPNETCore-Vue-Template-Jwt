using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Common.Exception
{
    public class AddAccountFailureException : SystemException
    {
        public AddAccountFailureException(string message) : base(message)
        {
            
        }
    }
}
