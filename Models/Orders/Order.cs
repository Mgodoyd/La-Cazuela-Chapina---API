namespace Api.Models

{
    public class Pedido
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public virtual Usuario User { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool Confirmed { get; set; } = false;
        public virtual ICollection<DetallePedido> Items { get; set; } = new List<DetallePedido>();
    }

} 