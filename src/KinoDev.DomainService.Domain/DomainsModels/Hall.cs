namespace KinoDev.DomainService.Domain.DomainsModels
{
    public class Hall : BaseEntity
    {
        public string Name { get; set; }

        public ICollection<ShowTime> ShowTimes { get; set; } = new List<ShowTime>();

        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
    }
}
