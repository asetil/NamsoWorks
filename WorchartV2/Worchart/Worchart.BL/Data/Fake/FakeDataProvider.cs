using System;
using System.Collections.Generic;

namespace Worchart.Data.Fake
{
    public class FakeDataProvider : IFakeDataProvider
    {
        public IEnumerable<T> GetFakeData<T>()
        {
            var entityName = typeof(T).Name;
            if (entityName == "Region")
            {
                //var list = new List<Region>();
                //list.Add(new Region() { ID = 1, Name = "İstanbul", ParentID = 0, SortOrder = "001", Status = Statuses.Active });
                //list.Add(new Region() { ID = 2, Name = "Ankara", ParentID = 0, SortOrder = "002", Status = Statuses.Active });
                //list.Add(new Region() { ID = 3, Name = "Kadıköy", ParentID = 1, SortOrder = "001001", Status = Statuses.Active });
                //list.Add(new Region() { ID = 4, Name = "Şişli", ParentID = 1, SortOrder = "001002", Status = Statuses.Active });
                //list.Add(new Region() { ID = 5, Name = "Hasanpaşa", ParentID = 3, SortOrder = "001001001", Status = Statuses.Active });
                //list.Add(new Region() { ID = 6, Name = "Mecidiyeköy", ParentID = 4, SortOrder = "001002001", Status = Statuses.Active });
                //list.Add(new Region() { ID = 7, Name = "Zincirlikuyu", ParentID = 4, SortOrder = "001002002", Status = Statuses.Active });
                //list.Add(new Region() { ID = 8, Name = "Fikirtepe", ParentID = 3, SortOrder = "001001002", Status = Statuses.Active });
                //list.Add(new Region() { ID = 9, Name = "Yenimahalle", ParentID = 2, SortOrder = "002001", Status = Statuses.Active });
                //list.Add(new Region() { ID = 10, Name = "Şentepe", ParentID = 9, SortOrder = "002001001", Status = Statuses.Active });
                //return list as IEnumerable<T>;
            }
            return new List<T>();
        }
    }
}