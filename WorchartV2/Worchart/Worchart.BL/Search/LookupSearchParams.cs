//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Worchart.Model;
//using Worchart.Util.Lookup;

//namespace Worchart.Search
//{
//    public class LookupSearchParams : SearchParams<Lookup>
//    {
//        public LookupType? LookupType { get; set; }
//        public override SearchHelper<Lookup> PrepareFilters()
//        {
//            var searchHelper = base.PrepareFilters();

//            if (LookupType.HasValue)
//            {
//                searchHelper.FilterBy(i => i.Type==LookupType.Value);
//            }

//            if (!string.IsNullOrEmpty(Keyword))
//            {
//                searchHelper.FilterBy(i => i.Name.ToLower().Contains(Keyword.ToLower()) || i.Value.Equals(Keyword.ToLower()));
//            }

//            return searchHelper;
//        }
//    }
//}
