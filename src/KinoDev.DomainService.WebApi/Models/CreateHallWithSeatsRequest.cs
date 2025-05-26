namespace KinoDev.DomainService.WebApi.Models
{
    public class CreateHallWithSeatsRequest
    {
        public string Name { get; set; }
        public int RowsCount { get; set; }
        public int SeatsCount { get; set; }
    }
}