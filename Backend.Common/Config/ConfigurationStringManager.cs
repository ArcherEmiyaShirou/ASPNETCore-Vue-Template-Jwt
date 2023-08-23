namespace my_project_backend.Config
{
    public class ConfigurationStringManager
    {
        public string DbConnectionString { get; set; } = Environment.GetEnvironmentVariable("DefaultDB:ConnStr") ?? throw new ArgumentNullException(nameof(DbConnectionString));
        public string JwtAudience { get; set; } = Environment.GetEnvironmentVariable("JWT:Audience") ?? throw new ArgumentNullException(nameof(JwtAudience));
        public string JwtIssuer { get; set; } = Environment.GetEnvironmentVariable("JWT:Issuer") ?? throw new ArgumentNullException(nameof(JwtIssuer));
        public string JwtSecretKey { get; set; } = Environment.GetEnvironmentVariable("JWT:SecretKey") ?? throw new ArgumentNullException(nameof(JwtSecretKey));
        public string PasswordEncode { get; set; } = Environment.GetEnvironmentVariable("PasswordEncode:Salt") ?? throw new ArgumentNullException(nameof(PasswordEncode));
        public string EmailAddress { get; set; } = Environment.GetEnvironmentVariable("163email_smtp_from") ?? throw new ArgumentNullException("enviroment variable '163email_smtp_from' didn't set!");
        public string EmailCredential { get; set; } = Environment.GetEnvironmentVariable("163email_smtp_code") ?? throw new ArgumentNullException("enviroment variable '163email_smtp_code' didn't set!");

        private ConfigurationStringManager()
        {

        }
        public static ConfigurationStringManager Instance { get; private set; } = new ConfigurationStringManager();
    }
}
