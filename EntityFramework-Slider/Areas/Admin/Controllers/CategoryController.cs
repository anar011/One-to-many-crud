using EntityFramework_Slider.Areas.Admin.ViewModels;
using EntityFramework_Slider.Data;
using EntityFramework_Slider.Helpers;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly AppDbContext _context;
        public CategoryController(ICategoryService categoryService,
                                  AppDbContext context)
        {
            _categoryService = categoryService;
            _context = context;
        }
        public async Task<IActionResult> Index(int page = 1,int take = 4)
        {
            //return View(await _categoryService.GetAll());


            List<Category> categories = await _categoryService.GetPaginatedDatas(page,take);

            List <CategoryListVM> mappedDatas = GetMappedDatas(categories);

            int pageCount = await GetPageCountAsync(take);

            Paginate<CategoryListVM> paginatedDatas = new(mappedDatas, page, pageCount);

            return View(paginatedDatas);

        }

        private async Task<int> GetPageCountAsync(int take)
        {
            var categoryCount = await _categoryService.GetCountAsync();
            return (int) Math.Ceiling((decimal) categoryCount / take);
        }

        private List<CategoryListVM> GetMappedDatas(List<Category> categories)
        {
            List<CategoryListVM> mappedDatas = new();

            foreach (var category in categories)
            {
                CategoryListVM categoryVM = new()
                {
                    Id = category.Id,
                    Name = category.Name
                };

                mappedDatas.Add(categoryVM);
            }
            return mappedDatas;

      
        }










        [HttpGet]
        public IActionResult Create()
        {
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {

            try
            {
                ///categori-larrin icerisinnen get gotur sec 1-cini
                ///( icerisindeki name-i trim.tolower et muqayiyise et gelen categoriyanin name-i ile) 
                ///eynidise exisdatan-i qaytarir {eyni deyilse null qaytarir}
                ///

                if(!ModelState.IsValid)
                {
                    return View();
                }

                var existData = await _context.Categories.FirstOrDefaultAsync(m => m.Name.Trim().ToLower() == category.Name.Trim().ToLower());

                if (existData is not null)    //// null deyilse ModelState-e gel error mesaj elave et ///
                {
                    ModelState.AddModelError("Name", "This data already exist");
                    return View();
                }

                int num = 1;
                int num2 = 0;
                int result = num / num2;


                throw new Exception("Model statemiz bu gun bizi yolda qoydu");


                //// Null deyilse yeni datai yarat /// 

                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));



            }
            catch (Exception ex)
            {

               
                return RedirectToAction("Error", new { msj = ex.Message });
            }
        
        }

        public IActionResult Error(string msj)
        {
            ViewBag.error = msj;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return BadRequest();

            Category category = await _context.Categories.FindAsync(id);

            if (category is null) return NotFound();    

             _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
   

        }

        [HttpPost]

        public async Task<IActionResult> SoftDelete(int? id)
        {
            if (id is null) return BadRequest();

            Category category = await _context.Categories.FindAsync(id);

            if (category is null) return NotFound();

            category.SoftDelete = true;
            await _context.SaveChangesAsync();
            return Ok();


        }


        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {



            if (id is null) return BadRequest();     // Eger Id null-sa BadRequest qaytar//


            Category category = await _context.Categories.FindAsync(id);

            if (category is null) return NotFound();



            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id,Category category)
        {

            try
            {

                if (!ModelState.IsValid)
                {
                    return View();
                }

                if (id is null) return BadRequest();


                Category dbCategory = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

                if (dbCategory is null) return NotFound();

                if (dbCategory.Name.Trim().ToLower() == category.Name.Trim().ToLower())
                {
                    return RedirectToAction(nameof(Index));
                }

                //dbCategory.Name = category.Name;

                _context.Categories.Update(category);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {

                return RedirectToAction("Error", new { msj = ex.Message });

            }

        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id is null) return BadRequest();     


            Category category = await _context.Categories.FindAsync(id);

            if (category is null) return NotFound();



            return View(category);
        }


    }

}
