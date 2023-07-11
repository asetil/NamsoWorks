using System.Linq;
using Aware.ECommerce.Model;
namespace WebMarket.Models
{
    public class BasketSummaryModel
    {
        public Basket Basket { get; set; }
        public BasketDrawMode DrawMode { get; set; }

        public bool HasItems
        {
            get { return Basket != null && Basket.Items.Any(); }
        }
    }

    public enum BasketDrawMode { 
        ForBasket=0,
        ForBasketSummary=1,
        ForOrder=2,
        ForOrderDetail=4
    }
}
