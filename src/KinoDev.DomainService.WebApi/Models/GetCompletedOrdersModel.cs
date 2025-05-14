namespace KinoDev.DomainService.WebApi.Models
{
    public class GetCompletedOrdersModel
    {
        public IEnumerable<Guid> OrderIds { get; set; }
    }
}