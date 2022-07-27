using System.Data.Entity;
using AspNetCoreHero.ToastNotification.Abstractions;
using Du_An.Extension;
using Du_An.Helpper;
using Du_An.Models;
using Du_An.ModelViews;
using Microsoft.AspNetCore.Mvc;

namespace Du_An.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly dbMarketssContext _context;
        public INotyfService _notyfService;

        public CheckoutController(dbMarketssContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        public List<CartItem> GioHang
        {
            get
            {
                var gh = HttpContext.Session.Get<List<CartItem>>("GioHang");
                if(gh == null){
                    gh = new List<CartItem>();
                }
                return gh;
            }
        }
        [HttpGet]
        [Route("checkout.html", Name="Checkout")]
        public IActionResult Index()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            var taiKhoanID = HttpContext.Session.GetString("CustomerId");
            MuaHangVM model = new MuaHangVM();
            if(taiKhoanID != null){
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x=> x.CustomerId == Convert.ToInt32(taiKhoanID));
                model.CustomerId = khachhang.CustomerId;
                model.FullName = khachhang.FullName;
                model.Email = khachhang.Email;
                model.Phone = khachhang.Phone;
                model.Address = khachhang.Address;
            }
            ViewBag.GioHang = cart;
            return View(model);
        }
        [HttpPost]
        [Route("checkout.html", Name="Checkout")]
        public IActionResult Index(MuaHangVM muaHang)
        {
            Console.WriteLine("aaaa");
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            var taiKhoanID = HttpContext.Session.GetString("CustomerId");
            MuaHangVM model = new MuaHangVM();
            Console.WriteLine(model.Address);
            if(taiKhoanID != null){
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x=> x.CustomerId == Convert.ToInt32(taiKhoanID));
                model.CustomerId = khachhang.CustomerId;
                model.FullName = khachhang.FullName;
                model.Email = khachhang.Email;
                model.Phone = khachhang.Phone;
                model.Address = khachhang.Address;
                khachhang.Address =muaHang.Address;
                _context.Update(khachhang);
                _context.SaveChanges();
            }
            
            try
            {
                if(ModelState.IsValid)
                {
                    Order donhang = new Order();
                    donhang.CustomerId = model.CustomerId;
                    donhang.Address = model.Address;
                    donhang.OrderDate = DateTime.Now;
                    donhang.TransactStatusId = 2; 
                    donhang.Deleted = false;
                    donhang.Paid = false;
                    donhang.Note = Utilities.StripHTML(model.Note);
                    donhang.TotalMoney =  Convert.ToInt32(cart.Sum(x=>x.TotalMoney));
                    
                    _context.Add(donhang);
                    _context.SaveChanges();

                    foreach (var item in cart)
                    {
                        OrderDetail orderDetail = new OrderDetail();
                        orderDetail.OrderId = donhang.OrderId;
                        orderDetail.ProductId = item.product.ProductId;
                        orderDetail.Amount = item.amount;
                        orderDetail.TotalMoney = donhang.TotalMoney;
                        orderDetail.Price = item.product.Price;
                        orderDetail.CreateDate = DateTime.Now;
                        _context.Add(orderDetail);
                    }
                    _context.SaveChanges();
                    HttpContext.Session.Remove("GioHang");
                    _notyfService.Success("Đơn hàng đặt thành công");
                    return RedirectToAction("Index", "Product");
                }

                
            }
            catch
            {

                Console.WriteLine("ddddd");
                ViewBag.GioHang = cart;
                return View(model);
            }
            ViewBag.GioHang = cart;
            return View(model);
        }
        // [Route("dat-hang-thanh-cong.html", Name="Success")]
        // public IActionResult Success()
        // {
        //     try
        //     {
        //         var taikhoanID= HttpContext.Session.GetString("CustomerId");
        //         if(string.IsNullOrEmpty(taikhoanID))
        //         {
        //             return RedirectToAction("Login", "Account", new {returnUrl = "/dat-hang-thanh-cong.html"});
        //         }
        //         var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x=>x.CustomerId==Convert.ToInt32(taikhoanID));
        //         var donhang = _context.Orders.Where(x=>x.CustomerId == Convert.ToInt32(taikhoanID)).OrderByDescending(x=>x.OrderDate).ToList();
        //         MuaHangSuccessVM successVM = new MuaHangSuccessVM();
        //         successVM.FullName = khachhang.FullName;
        //     }
        //     catch (System.Exception)
        //     {
                
        //         throw;
        //     }
        // }
    }
}