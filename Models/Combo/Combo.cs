namespace Api.Models

{
    public class Combo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public bool Editable { get; set; } = false;
        public ICollection<ProductoCombo> Productos { get; set; } = new List<ProductoCombo>();
    }

}