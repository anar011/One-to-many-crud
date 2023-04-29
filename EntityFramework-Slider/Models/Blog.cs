using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework_Slider.Models
{
    public class Blog:BaseEntity
    {
        [Required(ErrorMessage = "Don't be empty")]
        [StringLength(20, ErrorMessage = "The name is length must be max 20 character")]
        public string? Header { get; set; }
        public string? Description { get; set; }       
        public DateTime Date { get; set; }

        public string? Image { get; set; }

        [Required(ErrorMessage = "Don't be empty")]
        [NotMapped]                                      
        public IFormFile Photo { get; set; }



    }
}
