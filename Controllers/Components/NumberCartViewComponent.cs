using Du_An.Extension;
using Du_An.ModelViews;
using Microsoft.AspNetCore.Mvc;

namespace Du_An.Controllers.Components
{
    public class NumberCartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            int soluongsanpham = 0;
            if(cart != null){
                soluongsanpham = cart.Count();
            }
            return View(cart);
        }
    }
}