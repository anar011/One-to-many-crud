using EntityFramework_Slider.Models;

namespace EntityFramework_Slider.ViewModels
{
    public class HomeVM
    {
        public List<Slider> Sliders { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public SliderInfo SliderInfo { get; set; }

        public IEnumerable<ExpertHeader> ExpertHeaders { get; set; }

        public IEnumerable<Blog> Blogs { get; set; }

        public Experts Experts { get; set; }



    }
}
