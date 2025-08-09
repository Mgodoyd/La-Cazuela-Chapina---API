namespace Api.Models

{
    public class Pedido
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public bool Confirmado { get; set; } = false;
        public ICollection<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
    }

}