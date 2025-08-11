using System.ComponentModel.DataAnnotations;

namespace Api.DTOs

{
    public class BeverageDto : ProductBaseDto
    {
        public string Type { get; set; } = string.Empty;

        [Required]
        public string Sweetener { get; set; } = string.Empty;
        public string? Topping { get; set; }
        public string Size { get; set; } = "12oz";
    }
}