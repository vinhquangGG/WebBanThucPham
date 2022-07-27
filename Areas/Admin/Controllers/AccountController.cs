using Du_An.Models;
using Microsoft.AspNetCore.Mvc;

namespace Du_An.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly dbMarketssContext _context;

        public AccountController(dbMarketssContext context)
        {
            _context = context;
        }
        public IActionResult Login()
        {
            return View();
        }
    }
}