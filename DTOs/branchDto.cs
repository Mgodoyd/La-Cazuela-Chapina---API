using System.ComponentModel.DataAnnotations;

namespace Api.DTOs

{
    public class BranchDto
    {
        // public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;
        [Required]
        public string Phone { get; set; } = string.Empty;
    }
}