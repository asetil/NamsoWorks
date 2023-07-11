using System;
using System.Collections.Generic;
using Aware.Authenticate.Model;
using Aware.ECommerce.Model;
using Aware.Regional.Model;
using Aware.ECommerce.Enums;
using Aware.Notification;
using Aware.Util;
using Aware.Util.Enums;
using Aware.Util.Slider;

namespace Aware.Data.Fake
{
    public class FakeDataProvider : IFakeDataProvider
    {
        public IEnumerable<T> GetFakeData<T>()
        {
            var entityName = typeof(T).Name;
            if (entityName == "Region")
            {
                var list = new List<Region>();
                list.Add(new Region() { ID = 1, Name = "İstanbul", ParentID = 0, SortOrder = "001", Status = Statuses.Active });
                list.Add(new Region() { ID = 2, Name = "Ankara", ParentID = 0, SortOrder = "002", Status = Statuses.Active });
                list.Add(new Region() { ID = 3, Name = "Kadıköy", ParentID = 1, SortOrder = "001001", Status = Statuses.Active });
                list.Add(new Region() { ID = 4, Name = "Şişli", ParentID = 1, SortOrder = "001002", Status = Statuses.Active });
                list.Add(new Region() { ID = 5, Name = "Hasanpaşa", ParentID = 3, SortOrder = "001001001", Status = Statuses.Active });
                list.Add(new Region() { ID = 6, Name = "Mecidiyeköy", ParentID = 4, SortOrder = "001002001", Status = Statuses.Active });
                list.Add(new Region() { ID = 7, Name = "Zincirlikuyu", ParentID = 4, SortOrder = "001002002", Status = Statuses.Active });
                list.Add(new Region() { ID = 8, Name = "Fikirtepe", ParentID = 3, SortOrder = "001001002", Status = Statuses.Active });
                list.Add(new Region() { ID = 9, Name = "Yenimahalle", ParentID = 2, SortOrder = "002001", Status = Statuses.Active });
                list.Add(new Region() { ID = 10, Name = "Şentepe", ParentID = 9, SortOrder = "002001001", Status = Statuses.Active });
                return list as IEnumerable<T>;
            }
            else if (entityName == "Store")
            {
                var list = new List<Store>();
                list.Add(new Store()
                {
                    ID = 1,
                    Name = "Migros Sanal Market",
                    DisplayName = "Migros Sanal Market",
                    Description = "Türkiye’nin ilk sanal market uygulaması olan Migros Sanal Market, Türkiye’nin en büyük ve yaygın gıda e-ticaret sitesi olarak, 1997 yılından beri hizmet vermektedir. Migros Sanal Market’ten Migros mağazalarında satılan tüm ürünler, Migros mağazaları ile aynı fiyat ve avantajlarla 7 gün 24 saat boyunca internet üzerinden sipariş edilebilmektedir.",
                    ImageInfo = "Store/1.png",
                    MinOrderAmount = 20,
                    Status = Statuses.Active,
                    RegionInfo = ",5,6,7,8,",
                    WorkTimeInfo = "[600:1320;600:1320;540:1260;540:1260;540:1260;660:1080;1035:1035]"
                });

                list.Add(new Store()
                {
                    ID = 2,
                    Name = "Carrefour SA",
                    DisplayName = "Carrefour SA Merkez",
                    Description = "CarrefourSA, dünyadaki ilk mağazasını 15 Haziran 1963''te Fransa''da açan global perakende devi Carrefour’un; 1993 yılında Türkiye’deki ilk mağazasını İçerenköy’de açmasının ardından 1996 yılında gücünü Sabancı Holding ile birleştirmesiyle kuruldu. CarrefourSA, 2005 yılı Mayıs ayında Gima ve Endi''yi bünyesine katarak sektördeki büyüme hamlesini devam ettirdi.",
                    ImageInfo = "Store/50.png",
                    MinOrderAmount = 50,
                    Status = Statuses.Active,
                    RegionInfo = ",5,6,",
                    WorkTimeInfo = "[600:1350;420:1040;420:1040;420:1040;420:1040;420:1040;420:1040]"
                });
                return list as IEnumerable<T>;
            }
            else if (entityName == "Category")
            {
                var list = new List<Category>();
                list.Add(new Category { ID = 1, Name = "Meyve & Sebze", ParentID = 0, SortOrder = "001", ImageInfo = "[{'ID':1050,'Name':'Osman2','Path':'Category/1050.jpg','SortOrder':'234','Format':null,'RelationID':1,'RelationType':3,'Status':1,'IsImage':true,'Extension':'.jpg'},{'ID':1051,'Name':'234','Path':'Category/1051.jpg','SortOrder':'234','Format':null,'RelationID':1,'RelationType':3,'Status':1,'IsImage':true,'Extension':'.jpg'},{'ID':1052,'Name':'Deneme 01','Path':'Category/1052.jpg','SortOrder':'asd','Format':null,'RelationID':1,'RelationType':3,'Status':2,'IsImage':true,'Extension':'.jpg'},{'ID':1053,'Name':'wer','Path':'Category/1053.jpg','SortOrder':'345','Format':'34','RelationID':1,'RelationType':3,'Status':0,'IsImage':true,'Extension':'.jpg'}]", Status = Statuses.Active });
                list.Add(new Category { ID = 2, Name = "Et & Balik", ParentID = 0, SortOrder = "003", Status = Statuses.Active });
                list.Add(new Category { ID = 3, Name = "Süt & Kahvaltilik", ParentID = 0, SortOrder = "004", Status = Statuses.Active });
                list.Add(new Category { ID = 4, Name = "Gida & Yemek", ParentID = 0, SortOrder = "005", Status = Statuses.Active });
                list.Add(new Category { ID = 5, Name = "Abur Cubur", ParentID = 0, SortOrder = "006", Status = Statuses.Active });
                list.Add(new Category { ID = 6, Name = "Içecek", ParentID = 0, SortOrder = "002", Status = Statuses.Active });
                list.Add(new Category { ID = 7, Name = "Temizlik", ParentID = 0, SortOrder = "007", Status = Statuses.Active });
                list.Add(new Category { ID = 8, Name = "Kozmetik", ParentID = 0, SortOrder = "008", Status = Statuses.Active });
                list.Add(new Category { ID = 11, Name = "Meyve", ParentID = 1, SortOrder = "001002", ImageInfo = "[{'ID':1054,'Name':'asd234234234 234234234 2342342','Path':'Category/1054.jpg','SortOrder':'234','Format':'kk','RelationID':11,'RelationType':3,'Status':0,'IsImage':true,'Extension':'.jpg'}]", Status = Statuses.Active });
                list.Add(new Category { ID = 12, Name = "Sebze", ParentID = 1, SortOrder = "001003", Status = Statuses.Active });
                list.Add(new Category { ID = 13, Name = "Organik Meyve & Sebze", ParentID = 1, SortOrder = "001001", Status = Statuses.Active });
                list.Add(new Category { ID = 14, Name = "Erik", ParentID = 11, SortOrder = "001002001", Status = 0 });
                list.Add(new Category { ID = 15, Name = "Çilek", ParentID = 11, SortOrder = "001002002", Status = Statuses.Active });
                list.Add(new Category { ID = 16, Name = "Elma", ParentID = 11, SortOrder = "001002003", Status = Statuses.Active });
                list.Add(new Category { ID = 17, Name = "Kırmızı Et", ParentID = 2, SortOrder = "003001", Status = Statuses.Active });
                list.Add(new Category { ID = 18, Name = "Tavuk Eti", ParentID = 2, SortOrder = "003002", Status = Statuses.Active });
                list.Add(new Category { ID = 19, Name = "Balık Ürünleri", ParentID = 2, SortOrder = "003003", Status = Statuses.Active });
                list.Add(new Category { ID = 20, Name = "Et Şarküteri", ParentID = 2, SortOrder = "003004", Status = Statuses.Active });
                list.Add(new Category { ID = 22, Name = "Gazlı İçecekler", ParentID = 6, SortOrder = "002001", Status = Statuses.Active });
                list.Add(new Category { ID = 23, Name = "Meyve Suyu", ParentID = 6, SortOrder = "002002", Status = Statuses.Active });
                list.Add(new Category { ID = 24, Name = "Maden Suyu", ParentID = 6, SortOrder = "002003", Status = Statuses.Active });
                list.Add(new Category { ID = 25, Name = "Su", ParentID = 6, SortOrder = "002003", Status = Statuses.Active });
                list.Add(new Category { ID = 31, Name = "Omatik", ParentID = 0, SortOrder = "009", Status = Statuses.Passive });
                list.Add(new Category { ID = 32, Name = "Deneme", ParentID = 7, SortOrder = "007001", Status = Statuses.Active });
                return list as IEnumerable<T>;
            }
            else if (entityName == "User")
            {
                var list = new List<User>();
                list.Add(new User() { ID = 1, Name = "Osman Sokuoğlu", Email = "osman.sokuoglu@gmail.com", Password = "RkFkTJ2c5lxix/IdfuFHw3nx8IMSs1as7/94jPc4Dag=", Role = UserRole.SuperUser, Status = Statuses.Active });
                return list as IEnumerable<T>;
            }
            else if (entityName == "Address")
            {
                var list = new List<Address>();
                list.Add(new Address() { ID = 1, Name = "Ev Adresi", UserID = 1, CityID = 1, CountyID = 3, DistrictID = 5, Description = "Hürriyet Sok. Ayşe Hanım Apt. No:23/4", Phone = "5423334563", Status = Statuses.Active });
                list.Add(new Address() { ID = 2, Name = "İş Adresi", UserID = 1, CityID = 2, CountyID = 9, DistrictID = 7, Description = "Bayırçıkmazı Sok. Hayat Apt. No:42/6", Phone = "5423334563", Status = Statuses.Active });
                return list as IEnumerable<T>;
            }

            if (entityName == "Product")
            {
                var list = new List<Product>();
                list.Add(new Product()
                {
                    ID = 1,
                    Name = "Çaykur Rize Çay",
                    Brand = "Arçelik",
                    CategoryID = 14,
                    ImageInfo = "[{'ID':'1721','Path':'Product/23.png','SortOrder':'0'},{'ID':'1721','Path':'Product/24.png','SortOrder':'0'},{'ID':'1721','Path':'Product/25.png','SortOrder':'0'}]",
                    Status = Statuses.Active,
                    Unit = MeasureUnits.Unit,
                    PropertyInfo = "[{'ID':1,'ParentID':0,'Value':'25.12.2018','SortOrder':'1'},{'ID':14,'ParentID':13,'Value':'18','SortOrder':'1'},{'ID':15,'ParentID':13,'Value':'21','SortOrder':'3'},{'ID':16,'ParentID':13,'Value':'23','SortOrder':'2'}]"
                });
                list.Add(new Product() { ID = 2, Name = "Balküpü Küp Şeker", Brand = "Samsung", CategoryID = 4, ImageInfo = "[{'ID':'1721','Path':'Product/45.png','SortOrder':'0'}]", Status = Statuses.Active, Unit = MeasureUnits.Unit, PropertyInfo = "[{'ID':'8','Name':'Marka','Value':'Arçelik','SortOrder':'1','Type':'3'}]" });
                list.Add(new Product() { ID = 3, Name = "Cappy Vişne", Brand = "LG", CategoryID = 5, ImageInfo = "[{'ID':'1721','Path':'Product/28.png','SortOrder':'0'}]", Status = Statuses.Active, Unit = MeasureUnits.Unit, PropertyInfo = "[{'ID':'8','Name':'Marka','Value':'Arçelik','SortOrder':'1','Type':'3'}]" });
                list.Add(new Product() { ID = 4, Name = "Amasya Elma", Brand = "Apple", CategoryID = 6, ImageInfo = "[{'ID':'1721','Path':'Product/46.png','SortOrder':'0'}]", Status = Statuses.Active, Unit = MeasureUnits.Unit, PropertyInfo = "[{'ID':'8','Name':'Marka','Value':'Arçelik','SortOrder':'1','Type':'3'}]" });

                return list as IEnumerable<T>;
            }

            if (entityName == "StoreItem")
            {
                var list = new List<StoreItem>();
                list.Add(new StoreItem() { ID = 1, IsForSale = true, ProductID = 1, SalesPrice = 12.45M, ListPrice = 17M, Stock = -1, Status = Statuses.Active, StoreID = 1, DateModified = DateTime.Now.AddDays(-20), HasVariant = true });
                list.Add(new StoreItem() { ID = 2, IsForSale = true, ProductID = 2, SalesPrice = 9.87M, ListPrice = 15M, Stock = 90, Status = Statuses.Active, StoreID = 2, DateModified = DateTime.Now.AddDays(-20) });
                list.Add(new StoreItem() { ID = 2, IsForSale = true, ProductID = 1, SalesPrice = 11.87M, ListPrice = 15M, Stock = 90, Status = Statuses.Active, StoreID = 2, DateModified = DateTime.Now.AddDays(-20) });
                list.Add(new StoreItem() { ID = 3, IsForSale = false, ProductID = 3, SalesPrice = 5.11M, ListPrice = 6.78M, Stock = -1, Status = Statuses.Active, StoreID = 1, DateModified = DateTime.Now.AddDays(-20) });
                list.Add(new StoreItem() { ID = 4, IsForSale = true, ProductID = 4, SalesPrice = 5.23M, ListPrice = 6.50M, Stock = 121, Status = Statuses.Active, StoreID = 2, DateModified = DateTime.Now.AddDays(-20) });
                return list as IEnumerable<T>;
            }

            if (entityName == "Basket")
            {
                var list = new List<Basket>();
                list.Add(new Basket() { ID = 1, Name = "", UserID = 1, Status = Statuses.Active });
                list.Add(new Basket() { ID = 2, Name = "", UserID = 1, Status = Statuses.OrderedBasket });
                return list as IEnumerable<T>;
            }

            if (entityName == "BasketItem")
            {
                var list = new List<BasketItem>();
                list.Add(new BasketItem() { ID = 1, BasketID = 1, ItemID = 1, Price = 5.20M, ListPrice = 7.2M, Quantity = 2, GrossTotal = 10.4M, ProductID = 1, StoreID = 1, Status = Statuses.Active });
                list.Add(new BasketItem() { ID = 2, BasketID = 1, ItemID = 3, Price = 2.50M, ListPrice = 4.2M, Quantity = 5, GrossTotal = 12.5M, ProductID = 2, StoreID = 2, Status = Statuses.Active });
                list.Add(new BasketItem() { ID = 3, BasketID = 2, ItemID = 3, Price = 12.80M, ListPrice = 12.90M, Quantity = 3, GrossTotal = 38.4M, ProductID = 2, StoreID = 1, Status = Statuses.Active });
                list.Add(new BasketItem() { ID = 4, BasketID = 2, ItemID = 2, Price = 10.00M, ListPrice = 11.2M, Quantity = 1, GrossTotal = 10.0M, ProductID = 1, StoreID = 2, Status = Statuses.Active });
                return list as IEnumerable<T>;
            }

            if (entityName == "Order")
            {
                var list = new List<Order>();
                list.Add(new Order() { ID = 1, BasketID = 2, BasketTotal = 22.90M, BillingAddressID = 1, ShippingAddressID = 2, DiscountTotal = 0, GrossTotal = 22.90M, PaymentType = 2, ShipmentCost = 0, ShippingMethodID = 0, StoreID = 1, UserID = 1, Note = "deneme", Status = OrderStatuses.PreparingOrder });
                return list as IEnumerable<T>;
            }

            if (entityName == "ShippingMethod")
            {
                var list = new List<ShippingMethod>();
                list.Add(new ShippingMethod() { ID = 1, Name = "MNG Tüm Türkiye", Price = 7, RegionInfo = "-1", Description = "Tüm Türkiye", Status = Statuses.Active });
                list.Add(new ShippingMethod() { ID = 2, Name = "Sürat Hasanpaşa", Price = 5, RegionInfo = "5,7,8", Description = "Sürat Kargo", Status = Statuses.Active });
                list.Add(new ShippingMethod() { ID = 3, Name = "Yurtiçi Kargo - Şentepe", Price = 4.5M, RegionInfo = "10", Description = "Yurtiçi Kargo", Status = Statuses.Active });
                list.Add(new ShippingMethod() { ID = 4, Name = "Hasanpaşa Yurtiçi Kargo", Price = 4.8M, RegionInfo = "5,7,8", Description = "Yurtiçi Kargo", Status = Statuses.Active });
                list.Add(new ShippingMethod() { ID = 5, Name = "MNG Ankara İçi", Price = 2.5M, RegionInfo = "2", Description = "MNG Ankara İçi", Status = Statuses.Active });
                list.Add(new ShippingMethod() { ID = 6, Name = "Kadıköy FPS", Price = 3.2M, RegionInfo = "3", Description = "MNG Ankara İçi", Status = Statuses.Active });
                return list as IEnumerable<T>;
            }

            if (entityName == "SimpleItem")
            {
                var list = new List<SimpleItem>();
                list.Add(new SimpleItem() { ID = 1, Type = ItemType.SiteSettings, SubType = (int)SiteSettingsType.DisplayComments, SortOrder = "001", Value = "true" });
                list.Add(new SimpleItem() { ID = 2, Type = ItemType.SiteSettings, SubType = (int)SiteSettingsType.AllowNewComment, SortOrder = "002", Value = "true" });
                list.Add(new SimpleItem() { ID = 3, Type = ItemType.SiteSettings, SubType = (int)SiteSettingsType.AllowProductCompare, SortOrder = "003", Value = "true" });
                list.Add(new SimpleItem() { ID = 4, Type = ItemType.SiteSettings, SubType = (int)SiteSettingsType.AllowSocialLogin, SortOrder = "004", Value = "true" });
                list.Add(new SimpleItem() { ID = 5, Type = ItemType.SiteSettings, SubType = (int)SiteSettingsType.AllowSocialShare, SortOrder = "005", Value = "true" });
                list.Add(new SimpleItem() { ID = 6, Type = ItemType.OrderSettings, SubType = (int)OrderSettingsType.PaymentAtDoor, Title = "Kapıda Ödeme", SortOrder = "001", Value = "Siparişiniz hazırlandıktan sonra. Kargonuz kapınıza kadar getirilir ve ödemeyi kapıda nakit olarak yaparsınız.", Status = Statuses.Active });
                list.Add(new SimpleItem() { ID = 15, Type = ItemType.OrderSettings, SubType = (int)OrderSettingsType.AllowShipping, Title = "Kargo Kullan", SortOrder = "002", Value="true", Status = Statuses.Active });
                
                list.Add(new SimpleItem() { ID = 7, Type = ItemType.SiteSettings, SubType = (int)SiteSettingsType.FacebookApiKey, Value = "155231898222948", Status = Statuses.Active });
                list.Add(new SimpleItem() { ID = 8, Type = ItemType.SiteSettings, SubType = (int)SiteSettingsType.FacebookApiSecret, Value = "67678bf09c10b763b777862746f19bd4", Status = Statuses.Active });
                list.Add(new SimpleItem() { ID = 9, Type = ItemType.SiteSettings, SubType = (int)SiteSettingsType.GoogleApiKey, Value = "114984437885-ojvkjsu6u24jmvkambvg604p2754elvt.apps.googleusercontent.com", Status = Statuses.Active });
                list.Add(new SimpleItem() { ID = 10, Type = ItemType.SiteSettings, SubType = (int)SiteSettingsType.GoogleApiSecret, Value = "c3ymFy5ZudV6VVB5X2iVniPO", Status = Statuses.Active });
                list.Add(new SimpleItem() { ID = 11, Type = ItemType.SiteSettings, SubType = (int)SiteSettingsType.ReCaptchaSecret, Value = "6LcmxRYUAAAAAOfLlkJIkz4NQh1C1RrbxAYR-ceU", Status = Statuses.Active });
                
                return list as IEnumerable<T>;
            }

            if (entityName == "SliderItem")
            {
                var list = new List<SliderItem>
                {
                    new SliderItem()
                    {
                        ID = 12,
                        Type = SliderType.Main,
                        ImagePath =
                            "https://images.hepsiburada.net/assets/storefront/banners/30-05-2017_1496143262352_1.png",
                        Status = Statuses.Active
                    },
                    new SliderItem()
                    {
                        ID = 13,
                        Type = SliderType.Main,
                        ImagePath =
                            "https://images.hepsiburada.net/assets/storefront/banners/28-05-2017_1496054675262_1.png",
                        Status = Statuses.Active
                    },
                    new SliderItem()
                    {
                        ID = 14,
                        Type = SliderType.Main,
                        ImagePath =
                            "https://images.hepsiburada.net/assets/storefront/banners/30-05-2017_1496059020560_1.png",
                        Status = Statuses.Active
                    }
                };
                return list as IEnumerable<T>;
            }

            if (entityName == "PropertyValue")
            {
                var list = new List<PropertyValue>();
                list.Add(new PropertyValue() { ID = 1, Name = "Son Kullanma Tarihi", Status = Statuses.Active, Type = PropertyType.Text, SortOrder = "1", ParentID = 0 });
                list.Add(new PropertyValue() { ID = 2, Name = "Kırmızı", Status = Statuses.Active, Type = PropertyType.VariantOption, SortOrder = "1", ParentID = 2 });
                list.Add(new PropertyValue() { ID = 3, Name = "Sarı", Status = Statuses.Active, Type = PropertyType.VariantOption, SortOrder = "2", ParentID = 2 });
                list.Add(new PropertyValue() { ID = 4, Name = "Yeşil", Status = Statuses.Active, Type = PropertyType.VariantOption, SortOrder = "3", ParentID = 2 });
                list.Add(new PropertyValue() { ID = 5, Name = "S", Status = Statuses.Active, Type = PropertyType.VariantOption, SortOrder = "2", ParentID = 1 });
                list.Add(new PropertyValue() { ID = 6, Name = "M", Status = Statuses.Active, Type = PropertyType.VariantOption, SortOrder = "3", ParentID = 1 });
                list.Add(new PropertyValue() { ID = 7, Name = "L", Status = Statuses.Active, Type = PropertyType.VariantOption, SortOrder = "2", ParentID = 1 });
                list.Add(new PropertyValue() { ID = 8, Name = "XL", Status = Statuses.Active, Type = PropertyType.VariantOption, SortOrder = "1", ParentID = 1 });
                list.Add(new PropertyValue() { ID = 9, Name = "XXL", Status = Statuses.Active, Type = PropertyType.VariantOption, SortOrder = "2", ParentID = 1 });
                list.Add(new PropertyValue() { ID = 10, Name = "Ketçap", Status = Statuses.Active, Type = PropertyType.VariantOption, SortOrder = "2", ParentID = 3 });
                list.Add(new PropertyValue() { ID = 11, Name = "Mayonez", Status = Statuses.Active, Type = PropertyType.VariantOption, SortOrder = "2", ParentID = 3 });
                list.Add(new PropertyValue() { ID = 12, Name = "Ranch Sos", Status = Statuses.Active, Type = PropertyType.VariantOption, SortOrder = "2", ParentID = 3 });
                list.Add(new PropertyValue() { ID = 13, Name = "Telefon Özellikleri", Status = Statuses.Active, Type = PropertyType.PropertyGroup, SortOrder = "1", ParentID = 0 });
                list.Add(new PropertyValue() { ID = 14, Name = "Dahili Hafıza", Status = Statuses.Active, Type = PropertyType.Selection, SortOrder = "1", ParentID = 13 });
                list.Add(new PropertyValue() { ID = 15, Name = "Entegre Kamera", Status = Statuses.Active, Type = PropertyType.Selection, SortOrder = "1", ParentID = 13 });
                list.Add(new PropertyValue() { ID = 16, Name = "Ekran Çözünürlüğü", Status = Statuses.Active, Type = PropertyType.Selection, SortOrder = "1", ParentID = 13 });
               
                list.Add(new PropertyValue() { ID = 17, Name = "16 GB", Status = Statuses.Active, Type = PropertyType.PropertyOption, SortOrder = "1", ParentID = 14 });
                list.Add(new PropertyValue() { ID = 18, Name = "32 GB", Status = Statuses.Active, Type = PropertyType.PropertyOption, SortOrder = "3", ParentID = 14 });
                list.Add(new PropertyValue() { ID = 19, Name = "64 GB", Status = Statuses.Active, Type = PropertyType.PropertyOption, SortOrder = "2", ParentID = 14 });
               
                list.Add(new PropertyValue() { ID = 20, Name = "Var", Status = Statuses.Active, Type = PropertyType.PropertyOption, SortOrder = "1", ParentID = 15 });
                list.Add(new PropertyValue() { ID = 21, Name = "Yok", Status = Statuses.Active, Type = PropertyType.PropertyOption, SortOrder = "3", ParentID = 15 });
              
                list.Add(new PropertyValue() { ID = 21, Name = "496 x 648", Status = Statuses.Active, Type = PropertyType.PropertyOption, SortOrder = "3", ParentID = 16 });
                list.Add(new PropertyValue() { ID = 23, Name = "1440 x 2560", Status = Statuses.Active, Type = PropertyType.PropertyOption, SortOrder = "3", ParentID = 16 });
                list.Add(new PropertyValue() { ID = 24, Name = "1958 x 4096", Status = Statuses.Active, Type = PropertyType.PropertyOption, SortOrder = "1", ParentID = 16 });

                return list as IEnumerable<T>;
            }

            if (entityName == "VariantProperty")
            {
                var list = new List<VariantProperty>();
                list.Add(new VariantProperty() { ID = 1, Name = "Beden", DisplayMode = PropertyDisplayMode.ButtonGroup, IsRequired = true, TrackStock = true, Status = Statuses.Active, SortOrder = "2" });
                list.Add(new VariantProperty() { ID = 2, Name = "Renk", DisplayMode = PropertyDisplayMode.RadioGroup, IsRequired = true, TrackStock = true, Status = Statuses.Active, SortOrder = "1" });
                list.Add(new VariantProperty() { ID = 3, Name = "Sos Seçimi", DisplayMode = PropertyDisplayMode.Checkbox, IsRequired = false, TrackStock = false, Status = Statuses.Active, SortOrder = "3" });
                return list as IEnumerable<T>;
            }

            if (entityName == "VariantRelation")
            {
                var list = new List<VariantRelation>();
                list.Add(new VariantRelation() { ID = 1, RelationID = 1, RelationType = (int)RelationTypes.StoreItem, VariantID = 1, VariantValue = 6, Price = 0, Status = Statuses.Active });
                list.Add(new VariantRelation() { ID = 2, RelationID = 1, RelationType = (int)RelationTypes.StoreItem, VariantID = 1, VariantValue = 5, Price = 1, Status = Statuses.Active });
                list.Add(new VariantRelation() { ID = 3, RelationID = 1, RelationType = (int)RelationTypes.StoreItem, VariantID = 1, VariantValue = 8, Price = -2, Status = Statuses.Active });
                list.Add(new VariantRelation() { ID = 4, RelationID = 1, RelationType = (int)RelationTypes.StoreItem, VariantID = 2, VariantValue = 2, Price = 0, Status = Statuses.Active });
                list.Add(new VariantRelation() { ID = 5, RelationID = 1, RelationType = (int)RelationTypes.StoreItem, VariantID = 2, VariantValue = 4, Price = 1, Status = Statuses.Active });
                list.Add(new VariantRelation() { ID = 6, RelationID = 1, RelationType = (int)RelationTypes.StoreItem, VariantID = 2, VariantValue = 3, Price = 4, Status = Statuses.Active });
                list.Add(new VariantRelation() { ID = 7, RelationID = 1, RelationType = (int)RelationTypes.StoreItem, VariantID = 3, VariantValue = 11, Price = 0, Status = Statuses.Active });
                list.Add(new VariantRelation() { ID = 8, RelationID = 1, RelationType = (int)RelationTypes.StoreItem, VariantID = 3, VariantValue = 10, Price = 2.5M, Status = Statuses.Active });
                list.Add(new VariantRelation() { ID = 9, RelationID = 1, RelationType = (int)RelationTypes.StoreItem, VariantID = 3, VariantValue = 12, Price = 0.5M, Status = Statuses.Active });

                return list as IEnumerable<T>;
            }

            if (entityName == "VariantSelection")
            {
                var list = new List<VariantSelection>();
                list.Add(new VariantSelection() { ID = 1, RelationID = 1, RelationType = (int)RelationTypes.StoreItem, VariantCombination = "1:6,2:4", Price = 3.98M, Stock = 2 });
                list.Add(new VariantSelection() { ID = 2, RelationID = 1, RelationType = (int)RelationTypes.StoreItem, VariantCombination = "1:8,2:3", Price = 0, Stock = 101 });
                return list as IEnumerable<T>;
            }

            if (entityName == "Notification")
            {
                var result = new List<Notification.Notification>();
                result.Add(new Notification.Notification()
                {
                    ID = 1,
                    Target = NotificationTarget.All,
                    DisplayMode = NotificationDisplayMode.Popup,
                    PublishDate = DateTime.Now.AddDays(-3),
                    Expire = 5,
                    Name = "Reklam_1",
                    Status = Statuses.Active,
                    Content = "<img id='dfp-modal-image' src='https://securepubads.g.doubleclick.net/pcs/view?xai=AKAOjst_WkzbvIVuCvQr7yZYt-D2TISBKA4ANkKw-JFxeYrUx3eYrLREQpK7RNSvjBF418513TS8a_ZWiPuUwldLcHQOIHS-dYjn0ItJbhKMBcHodvNZ6CZ3XkHcRPj6XnUG4RYdxfwEY7FkjIetXdpoSg8kb9qx_PprYMSnt07ezX-EIOAxOuomBJ2a1hQUcAwPq0Aoi-f5asG6X_WEqFwgRpRkh2uvTQ5clKuK-IlSnCovg6TMdTu9C-d8vmLRKZjzWxTY&sig=Cg0ArKJSzIl2S4j79WucEAE&urlfix=1&adurl=https://tpc.googlesyndication.com/pagead/imgad?id=CICAgKDLr9iPoQEQARgBMggumbCYM19RpQ' style='width:800px; height:600px'>"
                });

                result.Add(new Notification.Notification()
                {
                    ID = 2,
                    Target = NotificationTarget.Members,
                    DisplayMode = NotificationDisplayMode.Popup,
                    PublishDate = DateTime.Now.AddDays(-2),
                    Expire = 3,
                    Name = "Reklam_2",
                    Status = Statuses.Active,
                    Content = "<img src='//i.hurimg.com/i/hurriyet/75/590x332/592681da2269a23b68d30b12.jpg' alt='Azmettirici anne' width='590' height='332'>"
                });

                result.Add(new Notification.Notification()
                {
                    ID = 3,
                    Target = NotificationTarget.NonMember,
                    DisplayMode = NotificationDisplayMode.Popup,
                    PublishDate = DateTime.Now.AddHours(-50),
                    Expire = 10,
                    Name = "Reklam_2",
                    Status = Statuses.Active,
                    Content = "<img src='https://tpc.googlesyndication.com/simgad/12994772394178363617'>"
                });
                return result as IEnumerable<T>;
            }
            return new List<T>();
        }
    }
}