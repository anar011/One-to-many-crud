using EntityFramework_Slider.Data;
using EntityFramework_Slider.Helpers;
using EntityFramework_Slider.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ExpertsController : Controller
    {

        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;                                // IWebHostEnvironment- vasitesi ile wwwrot-a catiriq //
        public ExpertsController(AppDbContext context,
                                IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Experts> experts = await _context.ExpertS.ToListAsync();

            return View(experts);
        }


        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();
            Experts experts = await _context.ExpertS.FirstOrDefaultAsync(m => m.Id == id);

            if (experts is null) return NotFound();

            return View(experts);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Experts experts)
        {
            try
            {
                if (!ModelState.IsValid)  // Eger input bosdursa ,her hansisa bir sekil secilmeyibse,hemin View-da qalsin /// Ve asagida error mesaj gostersin//
                {
                    return View();
                }

                var expertsData = await _context.ExpertS.FirstOrDefaultAsync(m => m.Name.Trim().ToLower() == experts.Name.Trim().ToLower());

                if (expertsData is not null)
                {
                    ModelState.AddModelError("Name", "This data already exist");
                    return View();
                }

                if (!experts.Photo.CheckFileType("image/"))
                {

                    ModelState.AddModelError("Photo", "File type must be image");
                    return View();

                }

                if (!experts.Photo.CheckFileSize(200))
                {

                    ModelState.AddModelError("Photo", "Image size must be max 200kb");
                    return View();

                }


                //Guid-datalari ferqli-ferqli yaratmaq ucun// 
                string fileName = Guid.NewGuid().ToString() + "_" + experts.Photo.FileName;               //Duzeltdiyin datani stringe cevir// 
                                                                                                             //Datanin adina photo - un adini birlesdir

                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

                //FileStream - bir fayli fiziki olaraq kompda harasa save etmek isteyirsense omda bir yayin(axin,muhit) yaradirsan ki,onun vasitesi ile save edesen.


                await FileHelper.SaveFileAsync(path, experts.Photo);

                //gelen sliderin image beraber etmek fileName-ye
                experts.Image = fileName;
                await _context.ExpertS.AddAsync(experts);
                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(Index));


            }
            catch (Exception ex)
            {

                throw;
            }


        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null) return BadRequest();

                Experts experts = await _context.ExpertS.FirstOrDefaultAsync(m => m.Id == id);

                if (experts is null) return NotFound();


                //img folderin icinde bele bir sekil varsa onu sil //
                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", experts.Image);

                FileHelper.DeleteFile(path);

                _context.ExpertS.Remove(experts);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));


            }
            catch (Exception)
            {

                throw;
            }


        }



        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return BadRequest();

            Experts experts = await _context.ExpertS.FirstOrDefaultAsync(m => m.Id == id);

            if (experts is null) return NotFound();


            return View(experts);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Experts experts)
        {
            try
            {
                if (id == null) return BadRequest();

                Experts dbExperts = await _context.ExpertS.FirstOrDefaultAsync(m => m.Id == id);

                if (dbExperts is null) return NotFound();

                if (!experts.Photo.CheckFileType("image/"))
                {

                    ModelState.AddModelError("Photo", "File type must be image");
                    return View(dbExperts);

                }

                if (!dbExperts.Photo.CheckFileSize(200))
                {

                    ModelState.AddModelError("Photo", "Image size must be max 200kb");
                    return View(dbExperts);

                }

                if (dbExperts.Name.Trim().ToLower() == experts.Name.Trim().ToLower())
                {
                    return RedirectToAction(nameof(Index));
                }

                if (dbExperts.Profession.Trim().ToLower() == experts.Profession.Trim().ToLower())
                {
                    return RedirectToAction(nameof(Index));
                }

                string oldPath = FileHelper.GetFilePath(_env.WebRootPath, "img", dbExperts.Image);


                FileHelper.DeleteFile(oldPath);  // Gel oldPath-i burdan sil

                string fileName = Guid.NewGuid().ToString() + "_" + experts.Photo.FileName; // sildikden sonra bir dene FileName yarat

                string newPath = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

                await FileHelper.SaveFileAsync(newPath, experts.Photo);



                dbExperts.Image = fileName;
                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(Index));



            }
            catch (Exception)
            {

                throw;
            }




        }



    }
}
