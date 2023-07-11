using System;
using System.Collections.Generic;
using Aware.Authenticate.Model;
using Aware.Data.Fake;
using Aware.ECommerce.Enums;
using Aware.ECommerce.Model;
using Aware.Util;
using CleanFramework.Business.Model;
using Aware.Util.Enums;

namespace CleanFramework.Data.Repository
{
    public class FakeDataProvider : IFakeDataProvider
    {
        public IEnumerable<T> GetFakeData<T>()
        {
            var entityName = typeof(T).Name;
            if (entityName == "Entry")
            {
                var entryList = new List<Entry>();
                entryList.Add(new Entry
                {
                    ID = 1,
                    CategoryID = 1,
                    UserID = 1,
                    Name = "Index Yapısı",
                    Content = "asdasdasd",
                    Summary = @"<p>
            Index tablo veya viewdeki bir veya daha fazla kolon üzerinde bir anahtar üretir ve bu anahtar bu verinin
            saklandigi yeri direk olarak isaret eder. Indexler sorgu sonucu getirilecek veriye ulasmak için gerekli olan
            veri okuma boyutunu büyük ölçüde azaltirlar. SQL Server’da 2 farklı Index yapısı mevcuttur. Clustered ve Non-Clustered Index.
        </p>",
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                    Status = Statuses.Active
                });
                entryList.Add(new Entry
                {
                    ID = 2,
                    CategoryID = 2,
                    UserID = 2,
                    Name = "Insert2 & Update2",
                    Content = "asdasdasd",
                    Summary = @"Elasticsearch is performing several operations when executing a filter: Find matching docs,
Build a bitset (The filter then builds a bitset--an array of 1s and 0s—that describes which documents
contain the term.), Cache the bitset(stored in memory). since we can use this in the future and skip
steps 1 and 2. This adds a lot of performance and makes filters very fast.",
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                    Status = Statuses.Active
                });

                entryList.Add(new Entry
                {
                    ID = 3,
                    CategoryID = 2,
                    UserID = 2,
                    Name = "NoSQL",
                    Content = @"<p>
            NoSQL sistemlerde tablo ve sütun kavramı yok. NoSQL sistemlerde yeni bir alana ihtiyacınız olduğu durumlarda
            kaydı direk eklemeniz yeterli olmaktadır. NoSQL sizin yerinize alanı oluşturur ve değeri kaydeder.
            Kayıtların maliyetsizce gerçekleşmesinin nedeni ise verilerin tablo ve sütunlarda saklanması
            yerine JSON ve XML formatına benzer yapıda saklanmasıdır.
        </p>
        <p>
            NoSQL, Fire and Forget prensibi ile çalıştığı için bankacılık, alışveriş gibi para üzerinden işlem yapılan kritik
            uygulamalarda kullanılmamalıdır.  Aksine verinin %100 önem arz etmediği durumlarda kullanımı daha uygundur.
            NoSQL veritabanı sistemleri okuma ve yazma performansları olarak göreceli olarak ilişkisel veritabanı sistemlerine
             göre daha performanslı olabilirler. yatay olarak genişletilebilirler.
        </p>
        <p>
            <b>Yatay Büyüme ve Dikey Büyüme Arasındaki Fark Nedir?</b><br />
            Dikey büyüme sistem kaynaklarınızın yetersiz kalması durumunda sistemi yenilemeniz veya sisteme ek donanım almak
            ile olur iken yatay büyümede ek bir sunucu alarak işlem yüklerini sunuculara bölmek ile sağlanır. Böylelikle
            Sunucuyu üst versiyonlara çıkarmak için çıkan karşılaşılan  maliyeti ek bir sunucu veya mainframe bir bilgisayarla
            sağlanmış olunur.
        </p>",
                    Summary = @"NoSQL sistemlerde tablo ve sütun kavramı yok. NoSQL sistemlerde yeni bir alana ihtiyacınız olduğu durumlarda
            kaydı direk eklemeniz yeterli olmaktadır. NoSQL sizin yerinize alanı oluşturur ve değeri kaydeder.
            Kayıtların maliyetsizce gerçekleşmesinin",
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                    Status = Statuses.Active
                });
                return entryList as IEnumerable<T>;
            }

            if (entityName == "Category")
            {
                var categoryList = new List<Category>();
                categoryList.Add(new Category { ID = 1, Name = "Angular", SortOrder = "001", Status = Statuses.Active });
                categoryList.Add(new Category { ID = 2, Name = "SQL", SortOrder = "002", Status = Statuses.Active });
                return categoryList as IEnumerable<T>;
            }

            if (entityName == "User")
            {
                var userList = new List<User>();
                userList.Add(new User { ID = 1, Name = "Osman Sokuoğlu", Email = "osman.sokuoglu@gmail.com", Password = "123123", Status = Statuses.Active, Role = UserRole.SuperUser});
                return userList as IEnumerable<T>;
            }
            return new List<T>();
        }
    }
}