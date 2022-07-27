using System.Data.Entity;
using AspNetCoreHero.ToastNotification.Abstractions;
using Du_An.Models;
using Microsoft.AspNetCore.Mvc;
using PagedList.Core;

namespace Du_An.Controllers
{
    public class PageController : Controller
    {
        private readonly dbMarketssContext _context;
        public INotyfService _notifyService { set; get; }

        public PageController(dbMarketssContext dbMarketssContext, INotyfService notyfService)
        {
            _context = dbMarketssContext;
            _notifyService = notyfService;
        }
        // GET: page/{Alias}
        [Route("/page/{Alias}", Name="Pages")]
        public IActionResult Details(string Alias)
        {
            if(string.IsNullOrEmpty(Alias)) return RedirectToAction("Index", "Home");
            var page = _context.Pages.AsNoTracking().SingleOrDefault(x=> x.Alias == Alias);
            if(page == null){
                return RedirectToAction("Index");
            }
            return View(page);
        }
    }
}