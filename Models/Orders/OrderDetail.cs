namespace Api.Models

{
    public class DetallePedido
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public virtual ProductoBase? Product { get; set; }
    }

}