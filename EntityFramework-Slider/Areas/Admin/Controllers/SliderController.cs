using EntityFramework_Slider.Areas.Admin.ViewModels;
using EntityFramework_Slider.Data;
using EntityFramework_Slider.Helpers;
using EntityFramework_Slider.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SliderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;                                // IWebHostEnvironment- vasitesi ile wwwrot-a catiriq //
        public SliderController(AppDbContext context, 
                                IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        { 
            IEnumerable<Slider> sliders = await _context.Sliders.ToListAsync();

            return View(sliders);

        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();
            Slider slider = await _context.Sliders.FirstOrDefaultAsync(m => m.Id == id);

            if (slider is null) return NotFound();

            return View(slider);
        }


        [HttpGet]
        public IActionResult Create () 
        { 
          return View();
         
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderCreateVM slider)
        {
            try
            {
                if (!ModelState.IsValid)  
                {
                    return View();
                }


                foreach (var photo in slider.Photos)
                {
                    if (!photo.CheckFileType("image/"))
                    {

                        ModelState.AddModelError("Photo", "File type must be image");
                        return View();

                    }

                    if (!photo.CheckFileSize(200))
                    {

                        ModelState.AddModelError("Photo", "Image size must be max 200kb");
                        return View();

                    }
                }

                foreach (var photo in slider.Photos)
                {


                    //Guid-datalari ferqli-ferqli yaratmaq ucun// 
                    string fileName = Guid.NewGuid().ToString() + "_" + photo.FileName;               //Duzeltdiyin datani stringe cevir// 
                                                                                                             //Datanin adina photo - un adini birlesdir

                    string path = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

                    //FileStream - bir fayli fiziki olaraq kompda harasa save etmek isteyirsense omda bir yayin(axin,muhit) yaradirsan ki,onun vasitesi ile save edesen.


                    await FileHelper.SaveFileAsync(path, photo);


                    Slider newSlider = new()    //Burada mutleq sekilde object yaradilmalidir. Yeni bir slayder yaradilmasi ucun onun ucun 
                    {                           //ViewModel-den gelene bearberlesdirmek ,add etmek dataBazadaki slidere ,save etmek bazaya.
                        Image = fileName
                    };

                    await _context.Sliders.AddAsync(newSlider);

                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));


            }
            catch (Exception)
            {

                throw;
            }
        }





        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(Slider slider)
        //{
        //    try
        //    {
        //        if(!ModelState.IsValid)  // Eger input bosdursa ,her hansisa bir sekil secilmeyibse,hemin View-da qalsin /// Ve asagida error mesaj gostersin//
        //        {
        //            return View();
        //        } 

        //        if (!slider.Photo.CheckFileType("image/"))
        //        {

        //            ModelState.AddModelError("Photo", "File type must be image");
        //            return View();
                
        //        }

        //        if(!slider.Photo.CheckFileSize(200))
        //        {

        //            ModelState.AddModelError("Photo", "Image size must be max 200kb");
        //            return View();

        //        }


        //        //Guid-datalari ferqli-ferqli yaratmaq ucun// 
        //        string fileName = Guid.NewGuid().ToString() + "_" + slider.Photo.FileName;               //Duzeltdiyin datani stringe cevir// 
        //                                                                                                 //Datanin adina photo - un adini birlesdir

        //        string path = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

        //        //FileStream - bir fayli fiziki olaraq kompda harasa save etmek isteyirsense omda bir yayin(axin,muhit) yaradirsan ki,onun vasitesi ile save edesen.


        //        await FileHelper.SaveFileAsync(path, slider.Photo);

        //        //gelen sliderin image beraber etmek fileName-ye
        //        slider.Image = fileName;
        //        await _context.Sliders.AddAsync(slider);
        //        await _context.SaveChangesAsync();


        //        return RedirectToAction(nameof(Index));


        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }

         
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null) return BadRequest();

                Slider slider = await _context.Sliders.FirstOrDefaultAsync(m => m.Id == id);

                if (slider is null) return NotFound();


                //img folderin icinde bele bir sekil varsa onu sil //
                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", slider.Image);

               FileHelper.DeleteFile(path);

                _context.Sliders.Remove(slider);

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

            Slider slider = await _context.Sliders.FirstOrDefaultAsync(m => m.Id == id);

            if (slider is null) return NotFound();

            return View(slider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Slider slider)
        {
            try
            {
                if (id == null) return BadRequest();

                Slider dbSlider = await _context.Sliders.FirstOrDefaultAsync(m => m.Id == id);

                if (dbSlider is null) return NotFound();

                if (!slider.Photo.CheckFileType("image/"))
                {

                    ModelState.AddModelError("Photo", "File type must be image");
                    return View(dbSlider);

                }

                if (!slider.Photo.CheckFileSize(200))
                {

                    ModelState.AddModelError("Photo", "Image size must be max 200kb");
                    return View(dbSlider);

                }

                string oldPath = FileHelper.GetFilePath(_env.WebRootPath, "img", dbSlider.Image);
              

                FileHelper.DeleteFile(oldPath);  // Gel oldPath-i burdan sil

                string fileName = Guid.NewGuid().ToString() + "_" + slider.Photo.FileName; // sildikden sonra bir dene FileName yarat

                string newPath =  FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

               await FileHelper.SaveFileAsync(newPath, slider.Photo);



                dbSlider.Image = fileName;             
                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(Index));


               
            }
            catch (Exception)
            {

                throw;
            }
             
           
         

        }



        [HttpPost]

        public async Task<IActionResult> SetStatus(int? id)
        {
            if (id is null) return BadRequest();

            Slider slider = await _context.Sliders.FirstOrDefaultAsync(m => m.Id == id);

            if (slider == null) return NotFound();

            slider.SoftDelete = !slider.SoftDelete;

            //if (slider.SoftDelete)
            //{
            //    slider.SoftDelete = false;
            //}
            //else
            //{
            //    slider.SoftDelete = true;
            //}

            await _context.SaveChangesAsync();

            return Ok(slider.SoftDelete);
        }


    }
}
