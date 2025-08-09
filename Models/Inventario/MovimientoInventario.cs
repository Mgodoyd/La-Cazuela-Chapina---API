namespace Api.Models

{
    public class MovimientoInventario
    {
        public int Id { get; set; }
        public int MateriaPrimaId { get; set; }
        public MateriaPrima MateriaPrima { get; set; } = null!;
        public decimal Cantidad { get; set; }
        public string TipoMovimiento { get; set; } = string.Empty; // Entrada, Salida, Merma
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
    }

}