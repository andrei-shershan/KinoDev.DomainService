namespace KinoDev.DomainService.Infrastructure.ConfigurationModels
{
    public class DomainDbSettings
    {
        public string MigrationAssembly { get; set; }

        public bool LogSensitiveData { get; set; }
    }
}