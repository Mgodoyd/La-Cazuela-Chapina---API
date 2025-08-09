namespace Api.Models

{
    public class Venta
    {
        public Guid  Id { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }
        public virtual Usuario User { get; set; } = null!;
        public decimal Total { get; set; }
        public virtual ICollection<DetalleVenta> Items { get; set; } = new List<DetalleVenta>();
    }

} 