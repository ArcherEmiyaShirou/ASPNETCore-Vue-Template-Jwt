using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Common.Utills
{
    public class Const
    {
        // mediatr
        public const string Event_SendEmail = nameof(Event_SendEmail);

        // Account_role
        public const string ROLE_USER = "user";
        public const string ROLE_ADMIN = "admin";

        //email code
        public const string VERIFY_EMAIL_LIMIT = nameof(VERIFY_EMAIL_LIMIT);    //time limit
        public const string VERIFY_EMAIL_DATA = nameof(VERIFY_EMAIL_DATA);

        //email type
        public const string EMAIL_TYPE_REGISTRATION = "registration";
        public const string EMAIL_TYPE_RESET = "reset";

        //JWT
        public const string JWT_BLACK_LIST = nameof(JWT_BLACK_LIST);
    }
}
