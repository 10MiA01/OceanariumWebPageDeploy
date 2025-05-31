using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Oceanarium.Validations;

namespace Oceanarium.Models
{

    public class Exibition
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        [Required]
        public bool IsPermanent { get; set; }

        public DateTime? StartDate { get; set; } // Только для временных
        
        public DateTime? EndDate { get; set; }
        [ValidateNever]
        public string? ImageUrl { get; set; }
    }
}
