using System.Data.Entity;
using Du_An.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SearchController : Controller
    {
        private readonly dbMarketssContext _context;

        public SearchController(dbMarketssContext context)
        {
            _context = context;
        }
        // [HttpPost]
        // public IActionResult FindProduct(string? keywordabc)
        // {
        //     Console.WriteLine(keywordabc);
        //     List<Product> ls = new List<Product>();
        //     if(string.IsNullOrEmpty(keywordabc) || keywordabc.Length < 1){
        //         Console.WriteLine("abcbc");
        //         return PartialView("ListProductsSearchPartial", null);
        //     }
        //     ls = _context.Products
        //                         .AsNoTracking()
        //                         .Include(a => a.Cat)
        //                         .Where(x => x.ProductName.Contains(keywordabc))
        //                         .OrderByDescending(x=>x.ProductName)
        //                         .Take(10)
        //                         .ToList();
        //     Console.WriteLine(ls.Count());
        //     if(ls == null){
        //         Console.WriteLine("keyword");
        //         return PartialView("ListProductsSearchPartial", null);
        //     }
        //     Console.WriteLine("bbbb");
        //     return PartialView("ListProductsSearchPartial", ls);
        // }
        [HttpPost]
        [HttpGet(Name="ListProductsSearchPartial")]
        public IActionResult FindProduct(string? keyword){
            Console.WriteLine(keyword);
            List<Product> ls = new List<Product>();
            ls = _context.Products
                                .AsNoTracking()
                                .Include(a => a.Cat)
                                .Where(x => x.ProductName.Contains(keyword))
                                .OrderByDescending(x=>x.ProductName)
                                .Take(10)
                                .ToList();
            if(ls == null){
                Console.WriteLine("keyword");
                return PartialView("ListProductsSearchPartial", null);
            }
            Console.WriteLine("bbbb");
            return PartialView("ListProductsSearchPartial", ls);
        }
    }
}