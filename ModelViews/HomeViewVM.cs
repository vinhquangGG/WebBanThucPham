using Du_An.Models;

namespace Du_An.ModelViews
{
    public class HomeViewVM
    {
        public List<TinDang> TinTucs{set;get;}
        public List<ProductHomeVM> Products{set;get;}
        public QuangCao quangCao{set;get;}

    }
}