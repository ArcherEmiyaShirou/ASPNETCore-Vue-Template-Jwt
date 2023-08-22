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
        Account FindAccountByNameOrEmail(string emailOrName);
        string RegistEmailVerifyCode(string type, string email, string address);
        string RegistEmailAccount(string EmailRegisterVO info);
        string resetEmailAccountPassword(EmailResetVO info);
        string resetConfirm(ConfirmResetVO info);
    }
}
