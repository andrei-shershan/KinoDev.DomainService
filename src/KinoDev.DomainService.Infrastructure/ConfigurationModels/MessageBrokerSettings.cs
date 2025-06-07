namespace KinoDev.DomainService.Infrastructure.ConfigurationModels
{
    public class MessageBrokerSettings
    {
        public Queues Queues { get; set; }
    }

    public class Queues
    {
        public string OrderFileCreated { get; set; }

        public string EmailSent { get; set; }

        public string OrderFileUrlAdded { get; set; }
    }
}