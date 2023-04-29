using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using EntityFramework_Slider.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.ContentModel;
using System.Diagnostics;

namespace EntityFramework_Slider.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IBasketService _basketService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IExpertService _expertService;
        private readonly IBlogService _blogService;
   



        public HomeController(AppDbContext context,
                              IBasketService basketService,
                              IProductService productService,
                              ICategoryService categoryService,
                              IExpertService expertService,
                              IBlogService blogService)

        {
            _context = context;
            _basketService = basketService;
            _productService = productService;
            _categoryService = categoryService;
            _expertService = expertService;
            _blogService = blogService;
        
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {

            List<Slider> sliders = await _context.Sliders.Where(m=>!m.SoftDelete).ToListAsync();

            SliderInfo sliderInfo = await _context.SliderInfos.FirstOrDefaultAsync();

            IEnumerable<Category> categories = await _categoryService.GetAll();

            IEnumerable<Product> products = await _productService.GetAll();
          
            IEnumerable<ExpertHeader> expertHeaders = await _expertService.GetAll();

            IEnumerable<Blog> blogs = await _blogService.GetAll();

            Experts experts = await _context.ExpertS.FirstOrDefaultAsync();




            HomeVM model = new()
            {
                Sliders = sliders,
                SliderInfo = sliderInfo,           
                Categories = categories,
                Products = products,
                ExpertHeaders = expertHeaders,
                Blogs = blogs,
                Experts = experts
                               
               
            };

            return View(model);
        }

        [HttpPost]
         
        public async Task<IActionResult>  AddBasket(int? id)
        {
            if (id is null) return BadRequest();

            Product? dbProduct = await _productService.GetById((int)id);

            if (dbProduct == null) return NotFound();


            List<BasketVM> basket = _basketService.GetBasketDatas();
       
            BasketVM? existProduct = basket?.FirstOrDefault(m => m.Id == dbProduct.Id);

            _basketService.AddProductToBasket(existProduct, dbProduct, basket);
           
            int basketCount = basket.Sum(m=>m.Count);

            return Ok(basketCount);
              
        }
    

    }

}