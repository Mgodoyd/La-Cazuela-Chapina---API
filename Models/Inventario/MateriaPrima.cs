namespace Api.Models

{
    public class MateriaPrima
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string UnidadMedida { get; set; } = string.Empty; // kg, litros, etc.
        public decimal StockMinimo { get; set; }
    }

}