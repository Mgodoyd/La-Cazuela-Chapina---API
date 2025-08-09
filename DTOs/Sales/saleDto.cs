namespace Api.DTOs

{
    public class SaleDto
    {
        // public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public Guid UserId { get; set; }
        public decimal Total { get; set; }
        public List<SaleDetailDto> Items { get; set; } = new List<SaleDetailDto>();
    }
}