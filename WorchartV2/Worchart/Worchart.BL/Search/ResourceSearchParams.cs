//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Worchart.Model;

//namespace Worchart.Search
//{
//    public class ResourceSearchParams : SearchParams<ResourceItem>
//    {
//        public string Code { get; set; }
//        public string Tr { get; set; }
//        public string En { get; set; }
//        public override SearchHelper<ResourceItem> PrepareFilters()
//        {
//            var searchHelper = base.PrepareFilters();

//            if (!string.IsNullOrEmpty(Code))
//            {
//                searchHelper.FilterBy(i => i.Code.ToLower().Contains(Code.ToLower()));
//            }

//            if (!string.IsNullOrEmpty(Tr))
//            {
//                searchHelper.FilterBy(i => i.Tr.ToLower().Contains(Tr.ToLower()));
//            }

//            if (!string.IsNullOrEmpty(En))
//            {
//                searchHelper.FilterBy(i => i.En.ToLower().Contains(En.ToLower()));
//            }

//            return searchHelper;
//        }
//    }
//}
