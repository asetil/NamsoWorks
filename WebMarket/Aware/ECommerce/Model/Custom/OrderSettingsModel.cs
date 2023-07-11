using System.Collections.Generic;
using System.Linq;
using Aware.Payment.Model;
using Aware.Util;
using Aware.ECommerce.Enums;
using Aware.Util.Enums;

namespace Aware.ECommerce.Model
{
    public class OrderSettingsModel
    {
        public bool AllowShipping { get; private set; }
        public bool AllowShippingTrack { get; private set; }
        public bool UseOOSPayment { get; private set; }
        public int DefaultPosID { get; private set; }
        public List<SimpleItem> PaymentTypes { get; private set; }
        public List<BankInfo> BankList { get; private set; }
        public List<PosDefinition> PosList { get; private set; }

        public SimpleItem GetPayment(PaymentType paymentType)
        {
            if (PaymentTypes != null && PaymentTypes.Any() && paymentType > 0)
            {
                var payment = PaymentTypes.FirstOrDefault(i => i.SubType == (int)paymentType && i.Status == Statuses.Active);
                return payment;
            }
            return null;
        }

        public BankInfo GetBank(int id)
        {
            if (BankList != null && BankList.Any() && id > 0)
            {
                return BankList.FirstOrDefault(i => i.ID == id);
            }
            return null;
        }

        public OrderSettingsModel Set(List<SimpleItem> settingList)
        {
            if (settingList != null && settingList.Any())
            {
                AllowShipping = settingList.Any(i => i.Type == ItemType.OrderSettings && i.SubType == (int)OrderSettingsType.AllowShipping && i.Status == Statuses.Active);
                AllowShippingTrack = settingList.Any(i => i.Type == ItemType.OrderSettings && i.SubType == (int)OrderSettingsType.AllowShippingTrack && i.Status == Statuses.Active);
                UseOOSPayment = settingList.Any(i => i.Type == ItemType.OrderSettings && i.SubType == (int)OrderSettingsType.UseOOSPayment && i.Status == Statuses.Active);
                PaymentTypes = settingList.Where(i => i.Type == ItemType.OrderSettings && i.SubType < (int)OrderSettingsType.AllowShipping && i.Status == Statuses.Active).ToList();
                DefaultPosID = 0;
                var paymentMethod = settingList.FirstOrDefault(i => i.Type == ItemType.OrderSettings && i.SubType == (int)OrderSettingsType.DefaultPos && i.Status == Statuses.Active);
                if (paymentMethod != null)
                {
                    DefaultPosID = paymentMethod.Value.Int();
                }
            }

            PaymentTypes = PaymentTypes ?? new List<SimpleItem>();
            return this;
        }

        public OrderSettingsModel SetBankList(List<BankInfo> bankList)
        {
            BankList = bankList;
            if (PaymentTypes != null && (bankList == null || !bankList.Any())) //Banka yoksa havale ile ödemede yok!!
            {
                PaymentTypes = PaymentTypes.Where(i => i.SubType != (int)OrderSettingsType.PaymentWithRemittance).ToList();
            }
            return this;
        }

        public OrderSettingsModel SetPosList(List<PosDefinition> posList)
        {
            PosList = posList;
            if (PaymentTypes != null && (posList == null || !posList.Any())) //Pos yoksa kredi kartı ile ödemede yok!!
            {
                PaymentTypes = PaymentTypes.Where(i => i.SubType != (int)OrderSettingsType.PaymentWithCreditCard).ToList();
            }
            return this;
        }
    }
}