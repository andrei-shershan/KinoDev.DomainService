namespace KinoDev.DomainService.Domain.DomainsModels
{
    public class Ticket : BaseEntity
    {
        public int ShowTimeId { get; set; }

        public int SeatId { get; set; }

        public int OrderId { get; set; }

        public ShowTime ShowTime { get; set; } = null!;

        public Seat Seat { get; set; } = null!;

        public Order Order { get; set; } = null!;
    }
}
