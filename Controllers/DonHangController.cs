using System.Data.Entity;
using AspNetCoreHero.ToastNotification.Abstractions;
using Du_An.Models;
using Du_An.ModelViews;
using Microsoft.AspNetCore.Mvc;

namespace Du_An.Controllers
{
    public class DonHangController : Controller
    {
        private readonly dbMarketssContext _context;
        public INotyfService _notyfService;

        public DonHangController(dbMarketssContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        [HttpPost]
        public IActionResult Details(int? id)
        {
            if(id == null){
                return NotFound();
            }
            try
            {
                var taikhoanID = HttpContext.Session.GetString("CustomerId");
                if(string.IsNullOrEmpty(taikhoanID)) return RedirectToAction("Login", "Account");
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x=>x.CustomerId == Convert.ToInt32(taikhoanID));
                if(khachhang == null){
                    return NotFound();
                }

                var donhang = _context.Orders.AsNoTracking()
                            .Include(x=>x.TransactStatus)
                            .Include(x=>x.Customer)
                            .FirstOrDefault(m=>m.OrderId == id && Convert.ToInt32(taikhoanID) == m.CustomerId);
                if(donhang == null)
                {
                    return NotFound();
                }
                var chitietdonhang = _context.OrderDetails.AsNoTracking()
                                    .Include(x=>x.Product)
                                    .Where(x=>x.OrderId == id)
                                    .OrderBy(x=>x.OrderDetailId).ToList();
                XemDonHang donHang = new XemDonHang();
                donHang.DonHang = donhang;
                donHang.ChiTietDonHang = chitietdonhang;
                var sanpham = _context.Products.AsNoTracking().ToList();
                ViewBag.Productss = sanpham;
                return View(donHang);
            }
            catch (System.Exception)
            {
                return NotFound();
            }
        }
    }
}