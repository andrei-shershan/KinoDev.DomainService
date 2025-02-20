namespace KinoDev.DomainService.Domain.DomainsModels
{
    public class Seat : BaseEntity
    {
        public int HallId { get; set; }

        public int Row { get; set; }

        public int Number { get; set; }

        public Hall Hall { get; set; } = null!;

        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
