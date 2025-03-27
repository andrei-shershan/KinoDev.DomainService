namespace KinoDev.DomainService.Domain.DomainsModels
{
    public class ShowTime : BaseIdEntity
    {
        public int MovieId { get; set; }

        public int HallId { get; set; }

        public DateTime Time { get; set; }

        public decimal Price { get; set; }

        public Movie Movie { get; set; } = null!;

        public Hall Hall { get; set; } = null!;

        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
