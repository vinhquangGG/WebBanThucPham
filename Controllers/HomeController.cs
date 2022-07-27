using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Du_An.Models;
using Du_An.ModelViews;
using System.Data.Entity;

namespace Du_An.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly dbMarketssContext _context;
    public HomeController(ILogger<HomeController> logger, dbMarketssContext context)
    {
        _logger = logger;
        _context = context;
    }
    [HttpGet]
    public IActionResult Index(string searchString)
    {
        Console.WriteLine(searchString);
        HomeViewVM model = new HomeViewVM();
        var lsProducts = _context.Products.AsNoTracking()
                        .Where(x=> x.Active == true)
                        .OrderByDescending(x=> x.DateCreated)
                        .ToList();
        if(!string.IsNullOrEmpty(searchString)){
            lsProducts = _context.Products.AsNoTracking()
                        .Where(x=> x.Active == true && x.ProductName.Contains(searchString))
                        .OrderByDescending(x=> x.DateCreated)
                        .ToList();
        }
        List<ProductHomeVM> lsProductViews = new List<ProductHomeVM>();
        var lsCats = _context.Categories
                    .AsNoTracking()
                    .Where(x=> x.Published==true)
                    .OrderByDescending(x=>x.Ordering)
                    .ToList();
        foreach (var item in lsCats)
        {
            ProductHomeVM productHome = new ProductHomeVM();
            productHome.category = item;
            productHome.lsProducts = lsProducts.Where(x=>x.CatId==item.CatId).ToList();
            lsProductViews.Add(productHome);
            
        }
        var quangcao = _context.QuangCaos
                        .AsNoTracking()
                        .FirstOrDefault(x=>x.Active==true);
        var tintuc = _context.TinDangs
                    .AsNoTracking()
                    .Where(x=> x.Published ==true && x.IsNewfeed == true)
                    .OrderByDescending(x=>x.CreatedDate)
                    .Take(3)
                    .ToList();
        model.Products = lsProductViews;
        model.quangCao = quangcao;
        model.TinTucs = tintuc;
        ViewBag.AllProducts = lsProducts;
        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }
    [Route("lien-he.html", Name="Contact")]
    public IActionResult Contact()
    {
        return View();
    }
    [HttpPost]
    [Route("lien-he.html", Name="Contact")]
    public IActionResult Contact(string con_name, string con_email, string con_message)
    {
        Location contact = new Location();
        contact.Name = con_name;
        contact.Slug = con_email;
        contact.NameWithType = con_message;
        if(!string.IsNullOrEmpty(con_name)){
            _context.Add(contact);
            _context.SaveChanges();
        }

        return View();
    }
    [Route("gioi-thieu.html", Name="About")]
    public IActionResult About()
    {
        return View();
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
