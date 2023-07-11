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

namespace CleanCode.DesignPattern
{

    public interface ICar
    {
        string BuildUp();
    }

    public class Araba : ICar
    {
        private string _name;
        private decimal _price;
        public Araba() { _name = "BMV X5"; _price = 200000; }
        public string BuildUp()
        {
            return _name + " " + _price + "TL değerinde üretim bandına girdi.";
        }
    }

    public class Bisiklet : ICar
    {
        private string _name;
        private string _corporate;
        public Bisiklet() { _name = "Bimeks 21 Vites"; _corporate = "NMS Mechanics"; }
        public string BuildUp()
        {
            return _name + " için " + _corporate + " firmasına sipariş geçildi.";

        }
    }

    /// <summary>
    /// ConcreteCreator
    /// </summary>
    public class FactoryMethod : IDesignPattern
    {
        public ICar GetCar(string type)
        {
            ICar car = null;
            if (type == "araba")
            {
                car = new Araba();
            }
            else if (type == "bisiklet")
            {
                car = new Bisiklet();
            }
            return car;
        }
    }

    public class FactoryClient
    {
        public void Test()
        {
            var factoryMethod = new FactoryMethod();
            var araba = factoryMethod.GetCar("araba");
            var bisiklet = factoryMethod.GetCar("bisiklet");

            Console.WriteLine("FACTORY DESIGN PATTERN");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Araba Talep: " + araba.BuildUp());
            Console.WriteLine("Bisiklet Talep : " + bisiklet.BuildUp());
        }
    }

    //    public enum EnumProduct
    //{
    //       PRODUCT_ONE(ProductOne.class.getName(),ProductOne.class),
    //       PRODUCT_TWO(ProductTwo.class.getName(),ProductTwo.class);

    //       private final String _productClassName;
    //       private final Class<?> _classType;

    //       private EnumProduct(String productClassName, Class<?> classType)
    //       {
    //             _productClassName = productClassName;
    //             _classType = classType;
    //       }

    //       public String getProductClassName()
    //       {
    //             return _productClassName;
    //       }

    //       public Class<?> getClassType()
    //       {
    //             return _classType;
    //       }
    //}
}
