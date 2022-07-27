using System.Data.Entity;
using System.Security.Claims;
using AspNetCoreHero.ToastNotification.Abstractions;
using Du_An.Extension;
using Du_An.Helpper;
using Du_An.Models;
using Du_An.ModelViews;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Du_An.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly dbMarketssContext _context;
        public INotyfService _notifyService { set; get; }

        public AccountController(dbMarketssContext dbMarketssContext, INotyfService notyfService)
        {
            _context = dbMarketssContext;
            _notifyService = notyfService;
        }
        public IActionResult ValidatePhone(string phone)
        {
            try
            {
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x=> x.Phone.ToLower() == phone.ToLower());
                if(khachhang != null){
                    return Json(data: "Số điện thoại: " + phone + " đã tồn tại");
                }
                return Json(data:true);
            }
            catch 
            {
                return Json(data:true);
            }
        }
        public IActionResult ValidateEmail(string email)
        {
            try
            {
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x=> x.Email.ToLower() == email.ToLower());
                if(khachhang != null){
                    return Json(data: "Số điện thoại: " + email + " đã tồn tại");
                }
                return Json(data:true);
            }
            catch 
            {
                return Json(data:true);
            }
        }
        [HttpGet]
        public IActionResult Dashboard()
        {
            var taiKhoanID = HttpContext.Session.GetString("CustomerId");
            if(taiKhoanID != null){
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x=>x.CustomerId==Convert.ToInt32(taiKhoanID));
                if(khachhang!=null){
                    var lsOrder = _context.Orders
                    .AsNoTracking()
                    .Include(x=>x.TransactStatus)
                    .Where(x=> x.CustomerId == khachhang.CustomerId)
                    .OrderByDescending(x=>x.OrderDate)
                    .ToList();
                    ViewBag.lsDonHang = lsOrder;
                    return View(khachhang);
                }
            }
            return RedirectToAction("Login");
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult DangKyTaiKhoan()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> DangKyTaiKhoan(RegisterVM taikhoan)
        {
            Console.WriteLine(taikhoan.Email);
            try
            {
                if(ModelState.IsValid)
                {
                    string salt = Utilities.GetRandomKey();
                    Customer khachhang = new Customer()
                    {
                        CustomerId = taikhoan.CustomerId,
                        FullName = taikhoan.FullName,
                        Phone = taikhoan.Phone,
                        Email = taikhoan.Email,
                        Password = (taikhoan.Password + salt.Trim()).ToMD5(),
                        Active = true,
                        Salt = salt,
                        CreateDate = DateTime.Now

                    };
                    try
                    {
                        _context.Add(khachhang);
                        await _context.SaveChangesAsync();
                        //Lưu mã khách hàng
                        HttpContext.Session.SetString("CustomerId", khachhang.CustomerId.ToString());
                        var taikhoanID = HttpContext.Session.GetString("CustomerId");
                        //Identity
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, khachhang.FullName),
                            new Claim("CustomerId", khachhang.CustomerId.ToString())
                        };
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        await HttpContext.SignInAsync(claimsPrincipal);
                        return RedirectToAction("Dashboard","Account");
                    }
                    catch
                    {
                        return RedirectToAction("DangKyTaiKhoan", "Account");
                    }
                }
                else{
                    return View(taikhoan);
                }
            }
            catch
            {
                return View(taikhoan);
            }
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("dang-nhap.html", Name ="DangNhap")]
        public IActionResult Login(string returnUrl = null)
        {
            var taiKhoanID = HttpContext.Session.GetString("CustomerId");
            if(taiKhoanID != null){
                return RedirectToAction("Dashboard", "Account");
            }
            
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("dang-nhap.html", Name ="DangNhap")]
        public async Task<IActionResult> Login(LoginViewModel customer)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    var account = _context.Accounts.AsNoTracking().SingleOrDefault(x=>x.Email.Trim() == customer.UserName);
                    if(account != null){
                        return Redirect("Admin/Home/Index");
                    }
                    var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x=>x.Email.Trim() == customer.UserName);
                    if(khachhang == null){
                        return View("DangKyTaiKhoan");
                    }
                    string pass = (customer.Password + khachhang.Salt.Trim()).ToMD5();
                    if(khachhang.Password != pass)
                    {
                        _notifyService.Error("Thông tin đăng nhập chưa chính xác");
                        return View(customer);
                    }
                    // Kiểm tra xem Account có bị disable?
                    if(khachhang.Active == false) return RedirectToAction("ThongBao", "Account");
                    Console.WriteLine(khachhang.CustomerId);
                    // Lưu Session MaKh
                    HttpContext.Session.SetString("CustomerId", khachhang.CustomerId.ToString());
                    var taikhoanID = HttpContext.Session.GetString("CustomerId");
                    //Identity
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, khachhang.FullName),
                        new Claim("CustomerId", khachhang.CustomerId.ToString())
                    };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                    _notifyService.Success("Đăng nhập thành công");
                    return RedirectToAction("DashBoard", "Account");
                }
            }
            catch
            {
                return RedirectToAction("DangKyTaiKhoan", "Acount");
            }
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Remove("CustomerId");
            // HttpContext.Session.Remove("DonHang");
            return RedirectToAction("Index","Home");
        }
        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                var taiKhoanID = int.Parse(HttpContext.Session.GetString("CustomerId"));
                if(taiKhoanID == null){
                    return RedirectToAction("Login", "Account");
                }
                if(ModelState.IsValid)
                {
                    var taiKhoan  = _context.Customers.Find(Convert.ToInt32(taiKhoanID));
                    if(taiKhoan == null) return RedirectToAction("Login", "Account");
                    var pass = (model.PasswordNow.Trim() + taiKhoan.Salt.Trim()).ToMD5();
                    
                    if(pass == taiKhoan.Password)
                    {
                        string newpass = (model.Password.Trim() + taiKhoan.Salt.Trim()).ToMD5();
                        taiKhoan.Password = newpass;
                        _context.Update(taiKhoan);
                        _context.SaveChanges();
                        _notifyService.Success("Đổi mật khẩu thành công");
                        return RedirectToAction("Dashboard", "Account");
                    } 
                }
            }
            catch
            {
               return RedirectToAction("Dashboard", "Account");
            }
            return RedirectToAction("Dashboard", "Account");
        }
    }
}