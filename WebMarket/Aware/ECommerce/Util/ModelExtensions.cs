using Aware.ECommerce.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Aware.Authenticate.Model;
using Aware.Util;
using Aware.Util.Model;
using Aware.ECommerce.Enums;

namespace Aware.ECommerce.Util
{
    public static class ModelExtensions
    {
        public static bool HasPermission(this User user, int permissionID)
        {
            if (!string.IsNullOrEmpty(user.Permissions))
            {
                return user.Permissions.Split(",").Contains(permissionID.ToString());
            }
            return false;
        }

        //public static bool IsViewableBy(this User user, User parent)
        //{
        //    if (parent != null)
        //    {
        //        if (parent.IsSuper || parent.ID == user.ID) { return true; }
        //        if (parent.Role == UserRole.Customer && user.ID == 0) { return true; }
        //        if (user.IsSuper || user.Status == Statuses.Deleted) { return false; }
        //        return !string.IsNullOrEmpty(user.StoreInfo) && user.StoreInfo.Trim(',').Split(',').All(i => parent.StoreInfo.IndexOf(i.S()) > -1);
        //    }
        //    return false;
        //}

        public static bool HasRegion(this Store store, int regionID)
        {
            return !string.IsNullOrEmpty(store.RegionInfo) && store.RegionInfo.IndexOf(regionID.ToString().S()) > -1;
        }

        public static Dictionary<string, string> GetWorkTimeInfo(this Store store)
        {
            Dictionary<string, string> workTimeInfo = new Dictionary<string, string>();
            string[] sArray = { "0:0", "0:0", "0:0", "0:0", "0:0", "0:0", "0:0" };

            if (store.ID > 0 && !string.IsNullOrEmpty(store.WorkTimeInfo))
            {
                sArray = store.WorkTimeInfo.Replace("[", "").Replace("]", "").Split(';');
            }

            for (int i = 1; i <= 7; i++)
            {
                int start = sArray[i - 1].Split(':')[0].Int();
                int finish = sArray[i - 1].Split(':')[1].Int();

                var value = "Servis yok!";
                if (finish - start > 1380) { value = "24 saat açık!"; }
                else if (finish - start >= 60)
                {
                    var shour = Math.Floor(start / 60F);
                    var sminute = start % 60;
                    var ehour = Math.Floor(finish / 60F);
                    var eminute = finish % 60;
                    value = string.Format("{0}{1}:{2}{3} - {4}{5}:{6}{7}", shour < 10 ? "0" : "", shour,
                        sminute < 10 ? "0" : "", sminute, ehour < 10 ? "0" : "", ehour, eminute < 10 ? "0" : "", eminute);
                }

                var day = Aware.Util.Common.GetDayName(i);
                workTimeInfo.Add(day, value);
            }
            return workTimeInfo;
        }

        public static ShippingMethod Clone(this ShippingMethod shippingMethod)
        {
            return new ShippingMethod()
            {
                Name = shippingMethod.Name,
                Price = shippingMethod.Price,
                Description = shippingMethod.Description,
                RegionInfo = shippingMethod.RegionInfo,
                Status = shippingMethod.Status
            };
        }

        public static List<Item> GetUnitList(this Product product, int quantityID = 0)
        {
            var unitList = new List<Item>();
            switch (product.Unit)
            {
                case MeasureUnits.Gram:
                    var units = new int[] { 50, 100, 200, 250, 400, 500, 750, 800, 900, 1000 };
                    for (int i = 0; i < units.Length; i++)
                    {
                        unitList.Add(new Item((i + 1), units[i].ToString(), units[i].ToString() + " gr"));
                    }
                    break;
                case MeasureUnits.Kg:
                    var funits = new float[] { 1, 1.5F, 2, 2.5F, 3, 3.5F, 4, 4.5F, 5, 5.5F, 6, 7, 7.5F, 8, 9, 10, 15, 20, 25, 30, 40, 50 };
                    for (int i = 0; i < funits.Length; i++)
                    {
                        unitList.Add(new Item((i + 1), funits[i].ToString(), funits[i].ToString() + " kg"));
                    }
                    break;
                default:
                    for (var i = 0; i < 10; i++)
                    {
                        unitList.Add(new Item((i + 1), (i + 1).ToString(), (i + 1).ToString() + " adet"));
                    }

                    if (quantityID > 10)
                    {
                        unitList.Add(new Item(quantityID, quantityID.ToString(), quantityID.ToString() + " adet"));
                    }
                    break;
            }
            return unitList;
        }

        public static decimal GetQuantity(this Product product, int quantityID)
        {
            try
            {
                decimal quantity = Convert.ToDecimal(product.GetUnitList().FirstOrDefault(i => i.ID == quantityID).Title);
                if (product.Unit == MeasureUnits.Gram) { quantity = quantity / 1000; }
                return quantity;
            }
            catch (Exception)
            {

            }
            return 0;
        }

        public static bool HasStock(this Product product, decimal value)
        {
            return product.Items != null && product.Items.Any(i => i.HasStock(value * product.UnitFactor));
        }

        public static bool HasStock(this StoreItem item, decimal quantity)
        {
            return item.IsForSale && (item.Stock == -1 || item.Stock >= quantity);
        }

        public static bool HasStock(this VariantSelection selection, decimal quantity)
        {
            return selection.Stock == -1 || selection.Stock >= quantity;
        }
    }
}