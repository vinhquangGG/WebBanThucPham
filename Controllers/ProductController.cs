using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Du_An.Models;
using Microsoft.AspNetCore.Mvc;
using PagedList.Core;

namespace Du_An.Controllers
{
    public class ProductController : Controller
    {
        private readonly dbMarketssContext _context;

        public ProductController(dbMarketssContext context)
        {
            _context = context;
        }
        [Route("/shop.html", Name="ShopProduct")]
        public IActionResult Index(int? page)
        {
            try
            {
                var pageNumber = page == null || page <= 0 ? 1 : page.Value;
                var pageSize = 10;
                var lsProducts = _context.Products.AsNoTracking().Include(x => x.Cat).OrderByDescending(x => x.DateCreated);
                var lsCat = _context.Categories.AsNoTracking().OrderByDescending(x=>x.CatId).ToList();
                PagedList<Product> models = new PagedList<Product>(lsProducts, pageNumber, pageSize);
                ViewBag.CurrentPage = pageNumber;
                var sp = _context.Products.AsNoTracking().Where(x=>x.Price >= 1000000).OrderByDescending(x=>x.Price).ToList();
                ViewBag.SP = sp;
                ViewBag.Category = lsCat;
                return View(models);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
            
        }
        [Route("/{Alias}", Name = "ListProduct")]
        public IActionResult List(string Alias, int page=1)
        {
            try
            {
                var pageSize = 10;
                var danhmuc = _context.Categories.AsNoTracking().SingleOrDefault(x => x.Alias == Alias);
                var lsProducts = _context.Products.AsNoTracking().Include(x => x.Cat)
                                .Where(x=> x.CatId == danhmuc.CatId)
                                .OrderByDescending(x => x.DateCreated);
                PagedList<Product> models = new PagedList<Product>(lsProducts, page, pageSize);
                ViewBag.CurrentPage = page;
                ViewBag.CurrentCat = danhmuc; 
                return View(models);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [Route("/{Alias}-{id}.html", Name = "ProductDetails")]
        public IActionResult Details(int? id)
        {
            try
            {
                var product = _context.Products.AsNoTracking().Include(x => x.Cat).FirstOrDefault(x=>x.ProductId==id);
                if(product == null){
                    return RedirectToAction("Index");
                }
                var lsProducts = _context.Products.AsNoTracking()
                            .Where(x=> x.CatId == product.CatId && x.ProductId != id && x.Active == true)
                            .OrderByDescending(x=>x.DateCreated)
                            .Take(4)
                            .ToList();
                ViewBag.SanPham = lsProducts;
                return View(product);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
                
            }
        }
    }
}