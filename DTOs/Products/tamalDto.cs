using System.ComponentModel.DataAnnotations;

namespace Api.DTOs

{
    public class TamalDto : ProductBaseDto
    {
        public string DoughType { get; set; } = string.Empty;

        [Required]
        public string Filling { get; set; } = string.Empty;

        [Required]
        public string Wrapper { get; set; } = string.Empty;

        [Required]
        public string SpiceLevel { get; set; } = string.Empty;
    }
}