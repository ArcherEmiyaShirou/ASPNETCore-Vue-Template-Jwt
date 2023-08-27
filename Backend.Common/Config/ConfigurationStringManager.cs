using Microsoft.Extensions.DependencyInjection;

namespace my_project_backend.Config
{
    public class ConfigurationStringManager
    {
        public string DbConnectionString { get; set; } = Environment.GetEnvironmentVariable("DefaultDB:ConnStr") ?? throw new ArgumentNullException("enviroment variable 'DefaultDB:ConnStr' didn't set!");
        public string JwtAudience { get; set; } = Environment.GetEnvironmentVariable("JWT:Audience") ?? throw new ArgumentNullException("enviroment variable 'JWT:Audience' didn't set!");
        public string JwtIssuer { get; set; } = Environment.GetEnvironmentVariable("JWT:Issuer") ?? throw new ArgumentNullException("enviroment variable 'JWT:Issuer' didn't set!");
        public string JwtSecretKey { get; set; } = Environment.GetEnvironmentVariable("JWT:SecretKey") ?? throw new ArgumentNullException("enviroment variable 'JWT:SecretKey' didn't set!");
        public string PasswordEncode { get; set; } = Environment.GetEnvironmentVariable("PasswordEncode:Salt") ?? throw new ArgumentNullException("enviroment variable 'PasswordEncode:Salt' didn't set!");
        public string EmailAddress { get; set; } = Environment.GetEnvironmentVariable("163email_smtp_from") ?? throw new ArgumentNullException("enviroment variable '163email_smtp_from' didn't set!");
        public string EmailCredential { get; set; } = Environment.GetEnvironmentVariable("163email_smtp_code") ?? throw new ArgumentNullException("enviroment variable '163email_smtp_code' didn't set!");

        private ConfigurationStringManager()
        {

        }
        public static ConfigurationStringManager Instance { get; private set; } = new ConfigurationStringManager();

        public static void ValifyConfig()
        {
            Instance ??= new ConfigurationStringManager();
        }
    }

    public static class ConfigurationStringManagerExtensions
    {
        public static IServiceCollection AddInitializer(this IServiceCollection services)
        {
            ConfigurationStringManager.ValifyConfig();

            return services;
        }
    }
}
