using System;
using System.Collections.Generic;
using System.Linq;
using Aware.ECommerce.Model;
using Aware.Search;
using Aware.Util.Enums;
using Aware.ECommerce.Enums;

namespace Aware.ECommerce.Search
{
    public class OrderSearchParams : SearchParams<Order>
    {
        public IEnumerable<int> StoreIDs { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PaymentType { get; set; }
        public OrderStatuses? OrderStatus { get; set; }

        public override SearchHelper<Order> PrepareFilters()
        {
            var searchHelper = SearchHelper;
            if (IDs != null && IDs.Any())
            {
                searchHelper.FilterBy(i => IDs.Contains(i.ID));
            }

            if (StoreIDs != null && StoreIDs.Any())
            {
                searchHelper.FilterBy(i => StoreIDs.Contains(i.StoreID));
            }

            if (StartDate != null && StartDate != DateTime.MinValue)
            {
                searchHelper.FilterBy(i => i.DateCreated >= StartDate);
            }

            if (EndDate != null && EndDate != DateTime.MinValue)
            {
                searchHelper.FilterBy(i => i.DateCreated <= EndDate);
            }

            if (OrderStatus.HasValue && OrderStatus.Value != OrderStatuses.None)
            {
                searchHelper.FilterBy(i => i.Status == OrderStatus);
            }

            if (PaymentType > 0)
            {
                searchHelper.FilterBy(i => i.PaymentType == PaymentType);
            }

            if (SearchHelper.SortList == null || !SearchHelper.SortList.Any())
            {
                SearchHelper.SortBy(i => i.DateCreated, true);
            }
            return searchHelper;
        }

        public static OrderSearchParams Init(Statuses status = Statuses.None, int page = 1, int size = 25)
        {
            var searchModel = new OrderSearchParams
            {
                Status = status,
            };

            searchModel.SetPaging(page, size);
            return searchModel;
        }
    }
}
