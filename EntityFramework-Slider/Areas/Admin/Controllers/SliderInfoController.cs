using EntityFramework_Slider.Areas.Admin.ViewModels;
using EntityFramework_Slider.Data;
using EntityFramework_Slider.Helpers;
using EntityFramework_Slider.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SliderInfoController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;                                // IWebHostEnvironment- vasitesi ile wwwrot-a catiriq //
        public SliderInfoController(AppDbContext context,
                                IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<SliderInfo> sliderInfos = await _context.SliderInfos.ToListAsync();

            return View(sliderInfos);
        }


        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();
            SliderInfo sliderInfo = await _context.SliderInfos.FirstOrDefaultAsync(m => m.Id == id);

            if (sliderInfo is null) return NotFound();

            return View(sliderInfo);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderInfo sliderInfo)
        {
            try
            {
                if (!ModelState.IsValid)  // Eger input bosdursa ,her hansisa bir sekil secilmeyibse,hemin View-da qalsin /// Ve asagida error mesaj gostersin//
                {
                    return View();
                }

                var existData = await _context.SliderInfos.FirstOrDefaultAsync(m => m.Title.Trim().ToLower() == sliderInfo.Title.Trim().ToLower());

                if (existData is not null)    
                {
                    ModelState.AddModelError("Name", "This data already exist");
                    return View();
                }

                if (!sliderInfo.Photo.CheckFileType("image/"))
                {

                    ModelState.AddModelError("Photo", "File type must be image");
                    return View();

                }

                if (!sliderInfo.Photo.CheckFileSize(200))
                {

                    ModelState.AddModelError("Photo", "Image size must be max 200kb");
                    return View();

                }


                //Guid-datalari ferqli-ferqli yaratmaq ucun// 
                string fileName = Guid.NewGuid().ToString() + "_" + sliderInfo.Photo.FileName;               //Duzeltdiyin datani stringe cevir// 
                                                                                                         //Datanin adina photo - un adini birlesdir

                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

                //FileStream - bir fayli fiziki olaraq kompda harasa save etmek isteyirsense omda bir yayin(axin,muhit) yaradirsan ki,onun vasitesi ile save edesen.


                await FileHelper.SaveFileAsync(path, sliderInfo.Photo);

                //gelen sliderin image beraber etmek fileName-ye
                sliderInfo.SignatureImage = fileName;
                await _context.SliderInfos.AddAsync(sliderInfo);
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

                SliderInfo sliderInfo = await _context.SliderInfos.FirstOrDefaultAsync(m => m.Id == id);

                if (sliderInfo is null) return NotFound();


                //img folderin icinde bele bir sekil varsa onu sil //
                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", sliderInfo.SignatureImage);

                FileHelper.DeleteFile(path);

                _context.SliderInfos.Remove(sliderInfo);

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

            SliderInfo sliderInfo = await _context.SliderInfos.FirstOrDefaultAsync(m => m.Id == id);

            if (sliderInfo is null) return NotFound();


            return View(sliderInfo);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, SliderInfo sliderInfo)
        {
            try
            {
                if (id == null) return BadRequest();

                SliderInfo dbSliderInfo = await _context.SliderInfos.FirstOrDefaultAsync(m => m.Id == id);

                if (dbSliderInfo is null) return NotFound();

                if (!sliderInfo.Photo.CheckFileType("image/"))
                {

                    ModelState.AddModelError("Photo", "File type must be image");
                    return View(dbSliderInfo);

                }

                if (!dbSliderInfo.Photo.CheckFileSize(200))
                {

                    ModelState.AddModelError("Photo", "Image size must be max 200kb");
                    return View(dbSliderInfo);

                }

                if (dbSliderInfo.Title.Trim().ToLower() == sliderInfo.Title.Trim().ToLower())
                {
                    return RedirectToAction(nameof(Index));
                }

                if (dbSliderInfo.Description.Trim().ToLower() == sliderInfo.Description.Trim().ToLower())
                {
                    return RedirectToAction(nameof(Index));
                }

                string oldPath = FileHelper.GetFilePath(_env.WebRootPath, "img", dbSliderInfo.SignatureImage);


                FileHelper.DeleteFile(oldPath);  // Gel oldPath-i burdan sil

                string fileName = Guid.NewGuid().ToString() + "_" + sliderInfo.Photo.FileName; // sildikden sonra bir dene FileName yarat

                string newPath = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

                await FileHelper.SaveFileAsync(newPath, sliderInfo.Photo);


               
                dbSliderInfo.SignatureImage = fileName;
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
