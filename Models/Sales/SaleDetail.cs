namespace Api.Models

{
    public class DetalleVenta
    {
        public Guid  Id { get; set; }
        public int SaleId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

} 