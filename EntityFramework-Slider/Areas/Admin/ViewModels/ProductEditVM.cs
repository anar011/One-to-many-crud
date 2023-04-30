using System.ComponentModel.DataAnnotations;

namespace EntityFramework_Slider.Areas.Admin.ViewModels
{
    public class ProductEditVM
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Price { get; set; }
        [Required]
        public int Count { get; set; }
        [Required]
        public string Description { get; set; }
        public int CategoryId { get; set; }
        [Required]
        public List<IFormFile> Photos { get; set; }
    }
}
