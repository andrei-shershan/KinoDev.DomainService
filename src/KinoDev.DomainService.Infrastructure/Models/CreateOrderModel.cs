namespace KinoDev.DomainService.Infrastructure.Models{
    public class CreateOrderModel{
        public int ShowTimeId { get; set; }
        public IEnumerable<int> SelectedSeatIds { get; set; }

        public string? Email { get; set; }

        public Guid? UserId { get; set; }
    }
}