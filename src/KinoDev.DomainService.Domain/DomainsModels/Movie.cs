namespace KinoDev.DomainService.Domain.DomainsModels
{
    public class Movie : BaseIdEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public DateOnly ReleaseDate { get; set; }

        public int Duration { get; set; }

        public string Url { get; set; }

        public ICollection<ShowTime> ShowTimes { get; set; } = new List<ShowTime>();
    }
}
