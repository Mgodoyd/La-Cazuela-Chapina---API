namespace Api.Models

{
    public class Venta
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }
        public Usuario User { get; set; } = null!;
        public decimal Total { get; set; }
        public ICollection<DetalleVenta> Items { get; set; } = new List<DetalleVenta>();
    }

} 