namespace Api.Models

{
    public class DetallePedido
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }

        public string? Status { get; set; } = "Solicitada";
        public decimal UnitPrice { get; set; }

        public virtual ProductoBase? Product { get; set; }
    }

}