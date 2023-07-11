using System.Collections.Generic;
using System.Linq;
using Aware.ECommerce.Model;
using Aware.Util;
using Aware.ECommerce.Enums;
using Aware.Util.Enums;

namespace WebMarket.Admin.Helper
{
    public static class Util
    {
        public static List<SimpleItem> ArrangeOrderSettings(List<SimpleItem> settings)
        {
            var result = new List<SimpleItem>();
            if (settings != null && settings.Any()) { result.AddRange(settings); }

            if (result.All(i => i.SubType != (int)OrderSettingsType.AllowShipping))
            {
                result.Add(new SimpleItem() { Title = "Kargo Kullanımı", Value = "false", Type = ItemType.OrderSettings, SubType = (int)OrderSettingsType.AllowShipping });
            }

            if (result.All(i => i.SubType != (int)OrderSettingsType.AllowShippingTrack))
            {
                result.Add(new SimpleItem() { Title = "Kargo Takibi", Value = "false", Type = ItemType.OrderSettings, SubType = (int)OrderSettingsType.AllowShippingTrack });
            }

            if (result.All(i => i.SubType != (int)OrderSettingsType.UseOOSPayment))
            {
                result.Add(new SimpleItem() { Title = "Ortak Ödeme Sayfası Kullan", Type = ItemType.OrderSettings, SubType = (int)OrderSettingsType.UseOOSPayment, Status = Statuses.Active });
            }

            if (result.All(i => i.SubType != (int)OrderSettingsType.DefaultPos))
            {
                result.Add(new SimpleItem() { Title = "Varsayılan POS", Type = ItemType.OrderSettings, SubType = (int)OrderSettingsType.DefaultPos, Status = Statuses.Active });
            }

            if (result.All(i => i.SubType != (int)OrderSettingsType.PaymentAtDoor))
            {
                result.Add(new SimpleItem() { Title = "Kapıda Ödeme", Type = ItemType.OrderSettings, SubType = (int)OrderSettingsType.PaymentAtDoor, Status = Statuses.Passive });
            }

            if (result.All(i => i.SubType != (int)OrderSettingsType.PaymentWithCreditCardAtDoor))
            {
                result.Add(new SimpleItem() { Title = "Kapıda Kredi Kartı ile", Type = ItemType.OrderSettings, SubType = (int)OrderSettingsType.PaymentWithCreditCardAtDoor, Status = Statuses.Passive });
            }

            if (result.All(i => i.SubType != (int)OrderSettingsType.PaymentWithRemittance))
            {
                result.Add(new SimpleItem() { Title = "Havale ile", Type = ItemType.OrderSettings, SubType = (int)OrderSettingsType.PaymentWithRemittance, Status = Statuses.Passive });
            }

            if (result.All(i => i.SubType != (int)OrderSettingsType.PaymentWithCreditCard))
            {
                result.Add(new SimpleItem() { Title = "Kredi Kartı ile", Type = ItemType.OrderSettings, SubType = (int)OrderSettingsType.PaymentWithCreditCard, Status = Statuses.Passive });
            }

            if (result.All(i => i.SubType != (int)OrderSettingsType.PaymentWithGarantiPay))
            {
                result.Add(new SimpleItem() { Title = "Garanti Pay ile", Type = ItemType.OrderSettings, SubType = (int)OrderSettingsType.PaymentWithGarantiPay, Status = Statuses.Passive });
            }
            return result;
        }
    }
}