namespace Api.Models

{
    public class InventarioItem
    {
        public int Id { get; set; }
        public int MateriaPrimaId { get; set; }
        public MateriaPrima MateriaPrima { get; set; } = null!;
        public decimal CantidadActual { get; set; }
    }

}