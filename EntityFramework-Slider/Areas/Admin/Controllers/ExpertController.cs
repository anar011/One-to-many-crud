using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]


    public class ExpertController : Controller
    {

        private readonly IExpertService _expertService;
        private readonly AppDbContext _context;
        public ExpertController(IExpertService expertService,
                                AppDbContext context )
        {
            _expertService = expertService;
            _context = context;
           
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<ExpertHeader> expertHeaders = await _expertService.GetAll();
            return View(expertHeaders);
        }



        [HttpGet]
        public IActionResult Create()
        {
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExpertHeader expert)
        {

            try
            {

                if (!ModelState.IsValid)
                {
                    return View();
                }
                

                var existData = await _context.ExpertHeaders.FirstOrDefaultAsync(m => m.Title.Trim().ToLower() == expert.Title.Trim().ToLower());

                if (existData is not null)  
                {
                    ModelState.AddModelError("Name", "This data already exist");
                    return View();
                }

                int num = 1;
                int num2 = 0;
                int result = num / num2;


                throw new Exception("Model statemiz bu gun bizi yolda qoydu");


               

                await _context.ExpertHeaders.AddAsync(expert);
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

            ExpertHeader expert = await _context.ExpertHeaders.FindAsync(id);

            if (expert is null) return NotFound();

            _context.ExpertHeaders.Remove(expert);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDelete(int? id)
        {
            if (id is null) return BadRequest();

            ExpertHeader expert = await _context.ExpertHeaders.FindAsync(id);

            if (expert is null) return NotFound();

            expert.SoftDelete = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


        }



        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return BadRequest();     


            ExpertHeader expert = await _context.ExpertHeaders.FindAsync(id);

            if (expert is null) return NotFound();



            return View(expert);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, ExpertHeader expert)
        {
            if (id is null) return BadRequest();    


            ExpertHeader dbExpert = await _context.ExpertHeaders.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

            if (dbExpert is null) return NotFound();

            if (dbExpert.Title.Trim().ToLower() == expert.Title.Trim().ToLower())
            {
                return RedirectToAction(nameof(Index));
            }


            if (dbExpert.Description.Trim().ToLower() == expert.Description.Trim().ToLower())
            {
                return RedirectToAction(nameof(Index));
            }

            _context.ExpertHeaders.Update(expert);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id is null) return BadRequest();


            ExpertHeader expert = await _context.ExpertHeaders.FindAsync(id);

            if (expert is null) return NotFound();



            return View(expert);
        }


    }
}
