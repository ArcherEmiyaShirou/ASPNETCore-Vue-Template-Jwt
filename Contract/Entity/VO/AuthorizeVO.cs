namespace Backend.Contract.Entity
{
    public class AuthorizeVO
    {
        public string Username { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
        public DateTime Expire { get; set; }
    }
}
