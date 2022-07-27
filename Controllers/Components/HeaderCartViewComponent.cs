using Du_An.Extension;
using Du_An.ModelViews;
using Microsoft.AspNetCore.Mvc;

namespace Du_An.Controllers.Components
{
    public class HeaderCartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            return View(cart);
        }
    }
}