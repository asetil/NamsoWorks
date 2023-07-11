using System;
using System.Collections.Generic;
using System.Linq;
using Aware.Util;
using Aware.ECommerce.Enums;

namespace Aware.ECommerce.Model
{
    public class DashboardStatisticModel
    {
        public IEnumerable<Store> Stores { get; set; }
        public IEnumerable<Order> Orders { get; set; }
        public decimal StockAlert { get; private set; }
        public decimal LowStock { get; private set; }
        public long ItemCount { get; private set; }
        public int TodayOrders { get; private set; }
        public int UnCompletedOrders { get; private set; }
        public int UnPaidOrders { get; private set; }
        public int WaitingApprovalOrders { get; private set; }
        public int PreparingOrders { get; private set; }
        public int DeliveredOrders { get; private set; }
        public int[] OrderCountData { get; private set; }
        public int[] OrderCompletedData { get; private set; }
        public decimal[] OrderPriceData { get; private set; }

        public DashboardStatisticModel CalculateStatistics()
        {
            OrderCountData = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            OrderCompletedData = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            OrderPriceData = new decimal[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            if (Stores != null && Stores.Any())
            {
                StockAlert = Stores.Sum(s => s.Statistic.NoStock + s.Statistic.RedStock);
                LowStock = Stores.Sum(s => s.Statistic.YellowStock);
                ItemCount = Stores.Sum(s => s.Statistic.ItemCount);
            }

            if (Orders != null && Orders.Any())
            {
                TodayOrders = Orders.Count(o => o.DateCreated > DateTime.Now.AddDays(-1) && o.DateCreated.DayOfYear == DateTime.Now.DayOfYear);
                UnCompletedOrders = Orders.Count(o =>o.Status== OrderStatuses.WaitingCustomerApproval);
                UnPaidOrders = Orders.Count(o => o.Status == OrderStatuses.WaitingPayment);
                WaitingApprovalOrders = Orders.Count(o => o.Status == OrderStatuses.WaitingApproval);
                PreparingOrders = Orders.Count(o => o.Status == OrderStatuses.PreparingOrder);
                DeliveredOrders = Orders.Count(o => o.Status == OrderStatuses.DeliveredOrder);

                var orders = Orders.Where(i => i.DateCreated > DateTime.Now.AddYears(-1)).GroupBy(g => g.DateCreated.Month);
                foreach (var orderGroup in orders)
                {
                    OrderCountData[orderGroup.Key - 1] = orderGroup.Count();
                    OrderCompletedData[orderGroup.Key - 1] = orderGroup.Count(c => c.Status == OrderStatuses.DeliveredOrder);
                    OrderPriceData[orderGroup.Key - 1] = orderGroup.Sum(s => s.GrossTotal);
                }
            }
            return this;
        }
    }
}