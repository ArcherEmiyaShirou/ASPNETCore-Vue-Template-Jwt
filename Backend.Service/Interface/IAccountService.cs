using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Contract.Entity;
using Backend.Contract.Entity.VO;

namespace Backend.Service.Interface
{
    public interface IAccountService
    {
        Task<Account?> FindAccountByNameOrEmail(string emailOrName);
        string RegistEmailVerifyCode(string type, string email, string address);
        Task<string> RegistEmailAccountAsync(EmailRegisterVO info);
        string resetEmailAccountPassword(EmailResetVO info);
        string resetConfirm(ConfirmResetVO info);
    }
}
