namespace Api.Models

{
    public class Pedido
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public Usuario User { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool Confirmed { get; set; } = false;
        public ICollection<DetallePedido> Items { get; set; } = new List<DetallePedido>();
    }

} 