namespace KinoDev.DomainService.Infrastructure.Models{
    public class CreateOrderModel{
        public int ShowTimeId { get; set; }
        public IEnumerable<int> SelectedSeatIds { get; set; }
    }
}