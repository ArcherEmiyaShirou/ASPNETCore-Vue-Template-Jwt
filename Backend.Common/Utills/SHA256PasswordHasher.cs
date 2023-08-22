using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using Backend.Common.Utills.Contract;

namespace Backend.Common.Utills
{
    public class SHA256PasswordHasher : IPasswordHasher
    {
        public async Task<string> HashPasswordAsync(string password)
        {
            return await Task<string>.Run<string>(() =>
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashedPassword = SHA256.HashData(passwordBytes);
                return Convert.ToBase64String(hashedPassword);
            });

        }

        public async Task<bool> VerifyPasswordAsync(string from, string target)
        {
            return await Task.Run<bool>(async () =>
            {
                string hashedFromPassword = await HashPasswordAsync(from);
                return target == hashedFromPassword;
            });
        }

    }
}
