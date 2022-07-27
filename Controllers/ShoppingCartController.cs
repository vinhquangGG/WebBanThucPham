using AspNetCoreHero.ToastNotification.Abstractions;
using Du_An.ModelViews;
using Du_An.Models;
using Microsoft.AspNetCore.Mvc;
using Du_An.Extension;

namespace Du_An.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly dbMarketssContext _context;
        public INotyfService _notyfService;

        public ShoppingCartController(dbMarketssContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        public List<CartItem> GioHang
        {
            get
            {
                var gh = HttpContext.Session.Get<List<CartItem>>("GioHang");
                if (gh == default(List<CartItem>))
                {
                    gh = new List<CartItem>();
                }
                return gh;
            }
        }
        /*
            1. Thêm mới sản phẩm vào giỏ hàng
            2. cập nhật lại số lượng sản phẩm trong giỏ hàng
            3. Xóa sản phẩm khỏi giỏ hàng
            4. Xóa luôn giỏ hàng
        */
        [HttpPost]
        [Route("api/cart/add")]
        public IActionResult AddToCart(int productID, int? amount)
        {
            Console.WriteLine(productID + " " + amount);
            List<CartItem> gioHang = GioHang;
            try
            {
                CartItem item = gioHang.SingleOrDefault(x => x.product.ProductId == productID);
                if (item != null)
                {
                    item.amount = item.amount + amount.Value;
                    HttpContext.Session.Set<List<CartItem>>("GioHang", gioHang);
                }
                else
                {
                    Product hh = _context.Products.SingleOrDefault(p => p.ProductId == productID);
                    item = new CartItem
                    {
                        amount = amount.HasValue ? amount.Value : 1,
                        product = hh
                    };
                    gioHang.Add(item); // Thêm vào giỏ hàng
                }
                // Lưu lại session
                HttpContext.Session.Set<List<CartItem>>("GioHang", gioHang);
                _notyfService.Success("Thêm sản phẩm thành công");
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
            // Thêm sản phẩm vào giỏ hàng

        }
        [HttpPost]
        [Route("api/cart/update")]
        public IActionResult UpdateCart(int productID, int? amount)
        {
            // Lấy giỏ hàng ra để xử lý
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            try
            {
                if(cart != null){
                    CartItem item = cart.SingleOrDefault(p=>p.product.ProductId == productID);
                    if(item != null && amount.HasValue) // Đã có sp -> cập nhật số lượng
                    {
                        item.amount = amount.Value;
                    }
                    //lưu lại session
                    HttpContext.Session.Set<List<CartItem>>("GioHang", cart);
                }
                return Json(new {success = true});
            }
            catch 
            {
                return Json(new {success = false});
            }
        }
        [HttpPost]
        [Route("api/cart/remove")]
        public IActionResult Remove(int productID)
        {
            
            try
            {
                List<CartItem> gioHang = GioHang;
                CartItem item = gioHang.SingleOrDefault(p=>p.product.ProductId == productID);
                if(item != null)
                {
                    gioHang.Remove(item);
                }
                HttpContext.Session.Set<List<CartItem>>("GioHang",gioHang);
                return Json(new {success = true});
            }
            catch 
            {
                return Json(new {success = false});
            }
        }
        [Route("cart.html", Name = "Cart")]
        public IActionResult Index()
        {
            return View(GioHang);
        }
    }
}