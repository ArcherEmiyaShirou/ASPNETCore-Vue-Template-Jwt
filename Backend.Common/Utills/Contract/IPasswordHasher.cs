namespace Backend.Common.Utills.Contract
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyPassword(string password);
    }
}
