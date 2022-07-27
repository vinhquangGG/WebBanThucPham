using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Du_An.Models;
using Microsoft.AspNetCore.Mvc;
using PagedList.Core;

namespace Du_An.Controllers
{
    public class BlogController : Controller
    {
        private readonly dbMarketssContext _context;
        public INotyfService _notifyService { set; get; }

        public BlogController(dbMarketssContext dbMarketssContext, INotyfService notyfService)
        {
            _context = dbMarketssContext;
            _notifyService = notyfService;
        }
        [Route("blogs.html", Name ="blog")]
        public async Task<IActionResult> Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 10;
            var lsTinDangs = _context.TinDangs.AsNoTracking().OrderByDescending(x => x.PostId);
            PagedList<TinDang> models = new PagedList<TinDang>(lsTinDangs, pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            float tmp = models.Count()/pageSize;
            if(tmp == (int)tmp){
                ViewBag.sumPages = (int)tmp;
            }
            else{
                ViewBag.sumPages = (int)tmp+1;
            }
            return View(models);
        }
        [Route("/tin-tuc/{Alias}-{id}.html", Name="TinDetails")]
        public IActionResult Details(int id)
        {
            var tinDang = _context.TinDangs.AsNoTracking().SingleOrDefault(x=> x.PostId == id);
            if(tinDang == null){
                return RedirectToAction("Index");
            }
            var lsBaiVietLienQuan = _context.TinDangs.AsNoTracking().Where(x => x.PostId != id && x.Published == true).OrderByDescending(x => x.CreatedDate).Take(3).ToList();
            ViewBag.BaiVietLienQuan = lsBaiVietLienQuan;
            return View(tinDang);
        }
    }
}