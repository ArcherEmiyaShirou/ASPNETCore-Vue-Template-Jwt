namespace Backend.Common.Utills.Contract
{
    public interface IPasswordHasher
    {
        Task<string> HashPasswordAsync(string password);
        Task<bool> VerifyPasswordAsync(string from, string target);
    }
}
