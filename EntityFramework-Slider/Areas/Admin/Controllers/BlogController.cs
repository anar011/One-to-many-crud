using EntityFramework_Slider.Data;
using EntityFramework_Slider.Helpers;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class BlogController : Controller
    {
        private readonly IBlogService _blogService;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public BlogController(IBlogService blogService,
                                  AppDbContext context,
                                  IWebHostEnvironment env)
        {
            _blogService = blogService;
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _blogService.GetAll());
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Blog blog)
        {

            try
            {
             

                if (!ModelState.IsValid)
                {
                    return View();
                }

                if (!blog.Photo.CheckFileType("image/"))
                {

                    ModelState.AddModelError("Photo", "File type must be image");
                    return View();

                }

                if (!blog.Photo.CheckFileSize(200))
                {

                    ModelState.AddModelError("Photo", "Image size must be max 200kb");
                    return View();

                }


                var existData = await _context.Blogs.FirstOrDefaultAsync(m => m.Header.Trim().ToLower() == blog.Header.Trim().ToLower());







                if (existData is not null)
                {
                    ModelState.AddModelError("Name", "This data already exist");
                    return View();
                }

                int num = 1;
                int num2 = 0;
                int result = num / num2;



                string fileName = Guid.NewGuid().ToString() + "_" + blog.Photo.FileName;               //Duzeltdiyin datani stringe cevir// 
                                                                                                       //Datanin adina photo - un adini birlesdir
                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);


                using (FileStream stream = new(path, FileMode.Create))
                {
                    await blog.Photo.CopyToAsync(stream);  // stream - bir axin(yayim,muhit)
                }

                blog.Image = fileName;
                await _context.Blogs.AddAsync(blog);
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

            try
            {
                if (id is null) return BadRequest();

                Blog blog = await _context.Blogs.FindAsync(id);

             

                if (blog is null) return NotFound();



                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", blog.Image);

                FileHelper.DeleteFile(path);

            


                _context.Blogs.Remove(blog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
                   





            }
            catch (Exception)
            {

                throw;
            }



         
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDelete(int? id)
        {
            if (id is null) return BadRequest();

            Blog blog = await _context.Blogs.FindAsync(id);

            if (blog is null) return NotFound();

            blog.SoftDelete = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


        }



        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {



            if (id is null) return BadRequest();     // Eger Id null-sa BadRequest qaytar//


            Blog blog = await _context.Blogs.FindAsync(id);

            if (blog is null) return NotFound();



            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Blog blog)
        {

            try
            {

                if (!ModelState.IsValid)
                {
                    return View();
                }

                if (id is null) return BadRequest();


                Blog dbBlog = await _context.Blogs.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

                if (dbBlog is null) return NotFound();

                if (dbBlog.Header.Trim().ToLower() == blog.Header.Trim().ToLower())
                {
                    return RedirectToAction(nameof(Index));
                }

                if (dbBlog.Description.Trim().ToLower() == blog.Description.Trim().ToLower())
                {
                    return RedirectToAction(nameof(Index));
                }


                if (dbBlog.Image.Trim().ToLower() == blog.Image.Trim().ToLower())
                {
                    return RedirectToAction(nameof(Index));
                }






                //dbCategory.Name = category.Name;

                _context.Blogs.Update(blog);

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


            Blog blog = await _context.Blogs.FindAsync(id);

            if (blog is null) return NotFound();



            return View(blog);
        }

    }
}
