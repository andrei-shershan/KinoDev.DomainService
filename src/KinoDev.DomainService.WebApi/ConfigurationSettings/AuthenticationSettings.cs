namespace KinoDev.DomainService.WebApi.ConfigurationSettings
{
    public class AuthenticationSettings
    {
        public string Secret { get; set; }

        public string Issuer { get; set; }

        public Audience Audiences { get; set; }
    }

    public class Audience
    {
        public string Internal { get; set; }
    }
}
