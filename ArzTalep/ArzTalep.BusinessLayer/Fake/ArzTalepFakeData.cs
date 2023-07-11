using ArzTalep.BL.Model;
using Aware.Data.Fake;
using Aware.Util.Enum;
using System.Collections.Generic;

namespace ArzTalep.BL.Fake
{
    public class ArzTalepFakeData : FakeDataProvider
    {
        public override IEnumerable<T> GetFakeData<T>()
        {
            var entityName = typeof(T).Name;
            if (entityName == typeof(Campaign).Name)
            {
                var list = new List<Campaign>
                {
                    new Campaign() { ID = 1, Name="Yudum 5Lt Ayçiçek Yağı", Status= StatusType.Active },
                    new Campaign() { ID = 2, Name="Prima Premium Care 5 Numara", Status= StatusType.Active },
                };
                return list as IEnumerable<T>;
            }
            else
                return base.GetFakeData<T>();
        }
    }
}
