//using EntityFramework_Slider.Data;
//using EntityFramework_Slider.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace EntityFramework_Slider.ViewComponents
//{
//    public class SliderViewComponent : ViewComponent
//    {
//        private readonly AppDbContext _context;
//        public SliderViewComponent(AppDbContext context)
//        {
//            _context = context;
//        }
//        public async Task<IViewComponentResult> InvokeAsync()
//        {
//            List<Slider> sliders = await _context.Sliders.ToListAsync();
//            return await Task.FromResult(View(sliders));
//        }
//    }
//}
