namespace KinoDev.DomainService.Infrastructure.Models
{
    public class CreateShowTimeRequest
    {
        public int MovieId { get; set; }
        public int HallId { get; set; }
        public DateTime Time { get; set; }
        public decimal Price { get; set; }
    }
}