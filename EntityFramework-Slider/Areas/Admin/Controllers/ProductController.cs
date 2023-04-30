using EntityFramework_Slider.Areas.Admin.ViewModels;
using EntityFramework_Slider.Data;
using EntityFramework_Slider.Helpers;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _env;    // wwwroot - a catmaq ucun
        private readonly AppDbContext _context;
        public ProductController(IProductService productService,
                                 ICategoryService categoryService,
                                 IWebHostEnvironment env,
                                AppDbContext context )
        {
            _productService = productService;
            _categoryService = categoryService;
            _env = env;
            _context = context;
        }
        //page- hansi sehifede oldugun// take-sehifede productu nece dene gostersin. //
        public async Task<IActionResult> Index(int page = 1, int take = 5)
        {
            List<Product> products = await _productService.GetPaginatedDatas(page, take);

            List<ProductListVM> mappedDatas = GetMappedDatas(products);

            int pageCount = await GetPageCountAsync(take);

            Paginate<ProductListVM> paginatedDatas = new(mappedDatas, page, pageCount); // mappedDatas-productlarin listi
                                                                                        // page- hal hazirda hasi sehifede oldugun
                                                                                        // pageCount - ne qeder count varsa onlar
            ViewBag.take = take;

            return View(paginatedDatas);
        }

        private async Task<int> GetPageCountAsync(int take)
        {
            var productCount = await _productService.GetCountAsync();
            return (int)Math.Ceiling((decimal)productCount / take);  // Celling- (decimal) istediyi ucun decimala cust edirik.
        }                                                              //oz metoduna gore ise (int)-e cust edirik 

        //private-de (async) yazilmama sebebi   Baza ile hec bir elaqesi omamasi 

        //yuxaridaki produktu bu metodun icerisine gondermek ucun//
        private List<ProductListVM> GetMappedDatas(List<Product> products)                   //Mapp = birlesdirmek,uygunlasdirmaq
        {
            // productlar list seklinde yazildi ve onnan object yaradildi ( bir classdan object yaratmaq ucun(istifade etmek ucun) instance almaq lazimdi) //

            List<ProductListVM> mappedDatas = new();

            foreach (var product in products)   //  elde olan produclari bir-bir elde etmek ucun
            {
                ProductListVM productsVM = new()  //tipleri ferqli oldugu ucun ProductListVM-den new yaradilir.Listden ayri denesinnen yaradilib,icin doldurub ona aid olan liste qoyulur //
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Count = product.Count,
                    CategoryName = product.Category.Name,
                    MainImage = product.Images.Where(m => m.IsMain).FirstOrDefault()?.Image
                };

                mappedDatas.Add(productsVM);
            }
            return mappedDatas;
        }




        [HttpGet]
        public async Task<IActionResult> Create()
        {


            ViewBag.categories = await GetCategoriesAsync();


            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateVM model)
        {
            try
            {
                ViewBag.categories = await GetCategoriesAsync();  // submit-etdikden sonra reflesh- etdikde category silinmemesi ucun 


                if (!ModelState.IsValid)
                {
                    return View(model); //model-i ,View-a gonderme sebebi create-eden zaman hansisa xana sef olduqda refles zamani butun xanalar silinmemesi ucun(yalnizca sef olan xanani yeniden yazmaq ucucn)
                }


                foreach (var photo in model.Photos)     //bir-bir sekilleri yoxlamaq ucun
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
                //instance - almamis obyektleri,List-leri bir-birine beraberlesdirmeyin
                List<ProductImage> productImages = new();


                foreach (var photo in model.Photos)   // sekilleri fiziki olaraq yaratmaq ucun (img folderinin icerisinde)//
                {


                    //Guid-datalari ferqli-ferqli yaratmaq ucun// 
                    string fileName = Guid.NewGuid().ToString() + "_" + photo.FileName;               //Duzeltdiyin datani stringe cevir// 
                                                                                                      //Datanin adina photo - un adini birlesdir
                                                                          //(img) - sekilleri fizi olaraq yaradiriq img folderin icerisinde 
                    string path = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

                    //FileStream - bir fayli fiziki olaraq kompda harasa save etmek isteyirsense onda bir yayin(axin,muhit) yaradirsan ki,onun vasitesi ile save edesen.


                    await FileHelper.SaveFileAsync(path, photo);   // sekli path edir,

                    ProductImage productImage = new()   // sekli gelir bura yazir (www.root-n icerisdindeki img-e sekli yazir)
                    {
                        Image = fileName
                    };

                    productImages.Add(productImage);   // sekli yazdiqdan sonra ,productImage-den gelir object yaradir //

                }


                productImages.FirstOrDefault().IsMain = true;   //sekli yuxarida olan List-e  [ List<ProductImage> productImages = new() ] elave edir

                decimal convertPrice = decimal.Parse(model.Price.Replace(".", ",")); //(Price) - da (.) noqte ve (,) vergul islemesi ucun.

                Product newProduct = new()
                {
                    Name = model.Name,
                    Price = convertPrice,
                    Count = model.Count,
                    Description = model.Description,
                    CategoryId = model.CategoryId,
                    Images = productImages
                };
                              //(AddRangeAsync)- Listi listin iceriseine qoymaq ucun hazir metod. 
                await _context.ProductImages.AddRangeAsync(productImages);
                await _context.Products.AddAsync(newProduct);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (Exception)
            {

                throw;
            }


        }



        private async Task<SelectList> GetCategoriesAsync()
        {
            IEnumerable<Category> categories = await _categoryService.GetAll();    // submit-etdikden sonra reflesh- etdikde category silinmemesi ucun 
            return new SelectList(categories, "Id", "Name");
        }



        [HttpGet]  //(Get) - metodunda productu yeni (id) - ni gedib bazadan getirmeliyik.
        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null) return BadRequest();

            Product product = await _productService.GetFullDataById((int)id);
           
            if(product == null) return NotFound();
            ViewBag.desc = Regex.Replace(product.Description, "<.*?>", String.Empty); // Productun Descriprion- hissesinin tag-le gelmemesi ucun.

            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteProduct(int? id)
        {
            Product product = await _productService.GetFullDataById((int)id);

            foreach (var item in product.Images)
            {

                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", item.Image);

                FileHelper.DeleteFile(path);
            }


            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id is null) return BadRequest();


            Product product = await _context.Products.FindAsync(id);

            if (product is null) return NotFound();



            return View(product);
        }






        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {



            if (id is null) return BadRequest();     // Eger Id null-sa BadRequest qaytar//


            Product product = await _context.Products.FindAsync(id);

            if (product is null) return NotFound();



            return View(product);
        }




    }
}
