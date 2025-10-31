namespace Api.Models

{
    public class DetalleVenta
    {
        public Guid Id { get; set; }
        public Guid SaleId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public virtual ProductoBase? Product { get; set; }

    }

}