using System.Security.Cryptography;
using Backend.Common.Utills.Contract;

namespace Backend.Common.Utills
{
    public class SHA256PasswordHasher :IPasswordHasher
    {
        public string HashPassword(string password)
        {

            return string.Empty;
        }

        public bool VerifyPassword(string password)
        {
            return true;
        }
    }
}
