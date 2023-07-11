/*
 * Birbirinden yapısal olarak farklı ancak aynı zamanda birçok karakteristik
 * özelliği ortak olan nesnelerin yönetimi, oluşturma kıstaslarının belirlenmesi ve yaratılması
 * için Factory Metodu kullanılır. İstenen tipte yeni nesne oluşturma sürecinin Factory sınıfına aktarılması
 * ile birlikte nesne üretme ve initialize etme süreci client tan soyutlanmış olur,
 * bu sayede client; uygulama içerisinde tamamen kendi rolüne odaklanmış olur.
 * 
 * Kısaca faktry kendsinden istenilen tipte nesne üretilmesini sağlar. Örneğin iki ayrı galeri için 
 * bi program yazdınız. Araba galerisi araba satışı yapıyor, Bisiklet galerisi de bisiklet. Siz müşteriye uygun 
 * ürünü oluşturup müşterinin BuildUp() metoduyla kendi seçiminin üretilmesini tetikleyeceksiniz.
 * 
 * In a sentence we are capable of using Factory Method Pattern where the requirements are frequently changing. 
 * 
 */


using System;
using System.Collections.Generic;
using System.Threading;

namespace CleanCode.DesignPattern
{
    public class Facade
    {
        private ProductService _productService = new ProductService();
        private BasketService _basketService = new BasketService();
        private VariantService _variantService = new VariantService();

        public bool AddToBasket(int userID, int productID, int quantity, string variantSelection)
        {
            var product = _productService.GetProduct(productID);
            var basketID = _basketService.GetUserBasket(userID);
            var variantItems = _variantService.GetVariants(variantSelection);

            _basketService.AddToBasket(basketID,product,quantity,variantItems);
            return true;
        }
    }

    public class ProductService
    {
        public object GetProduct(int productID)
        {
            return new object();
        }
    }

    public class BasketService
    {
        public int GetUserBasket(int userID)
        {
            return 0;
        }

        public object AddToBasket(int basketID,object product,int quantity,List<object> variantItems)
        {
            throw new NotImplementedException();
        }
    }

    public class VariantService
    {
        public List<object> GetVariants(string variantSelection)
        {
            return new List<object>();
        }
    }
}
