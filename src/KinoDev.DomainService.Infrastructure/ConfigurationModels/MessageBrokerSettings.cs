namespace KinoDev.DomainService.Infrastructure.ConfigurationModels
{
    public class MessageBrokerSettings
    {
        public Topics Topics { get; set; }

        public Queues Queues { get; set; }
    }

    public class Topics
    {
        public string OrderFileCreated { get; set; }

        public string OrderFileUrlAdded { get; set; }

        public string EmailSent { get; set; }
    }

    public class Queues
    {
        public string OrderFileCreated { get; set; }

        public string EmailSent { get; set; }
    }
}