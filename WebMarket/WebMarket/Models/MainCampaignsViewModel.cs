using System.Collections.Generic;
using Aware.ECommerce.Model;
using Aware.Util.Model;

namespace WebMarket.Models
{
    public class MultiSliderModel:Slider
    {
        public MultiSliderModel(string id, string css = "",int switchTime = 5000)
            : base(css, switchTime, id) {}

        public string Title { get; set; }
        public List<Product> Products  { get; set; }
        public int ItemCount { get; set; }
        public bool LazyLoad { get; set; }
        
    }
}