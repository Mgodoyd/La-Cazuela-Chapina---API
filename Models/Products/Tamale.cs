namespace Api.Models

{
    public class Tamal : ProductoBase
    {
        public string DoughType { get; set; } = string.Empty; // amarillo, blanco, arroz
        public string Filling { get; set; } = string.Empty;  // cerdo, pollo, vegetariano, etc.
        public string Wrapper { get; set; } = string.Empty; // plátano, tusa
        public string SpiceLevel { get; set; } = string.Empty; // sin, suave, chapín
    }

}