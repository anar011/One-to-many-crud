using System.ComponentModel.DataAnnotations;

namespace EntityFramework_Slider.Models
{
    public class ExpertHeader:BaseEntity
    {
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
