namespace Api.Models

{
    public class ComandoVoz
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public string TextoTranscrito { get; set; } = string.Empty;
        public string Intent { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
    }

}