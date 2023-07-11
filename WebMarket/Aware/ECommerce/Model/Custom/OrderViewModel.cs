using System.Collections.Generic;
using System.Linq;
using Aware.Authenticate.Model;
using Aware.Regional.Model;
using Aware.ECommerce.Enums;

namespace Aware.ECommerce.Model
{
    public class OrderViewModel
    {
        public Order Order { get; set; }
        public User User { get; set; }
        public Basket Basket { get; set; }
        public IEnumerable<Address> AddressList { get; set; }
        public List<ShippingMethod> ShippingMethods { get; set; }
        public int SelectedShippingAddress { get; set; }
        public int SelectedBillingAddress { get; set; }
        public bool AllowAddressSelection { get; set; }
        public OrderSettingsModel Settings { get; set; }
        public string  PaymentTypeDesc{ get; set; }


        public ShippingMethod ShippingMethod
        {
            get
            {
                ShippingMethod result = null;
                if (ShippingMethods != null && ShippingMethods.Any() && Order != null)
                {
                    result = ShippingMethods.FirstOrDefault(i => i.ID == Order.ShippingMethodID);
                }
                return result ?? new ShippingMethod();
            }
        }

        public Address ShippingAddress
        {
            get
            {
                Address result = null;
                if (AddressList != null && AddressList.Any() && Order != null)
                {
                    result = AddressList.FirstOrDefault(i => i.ID == Order.ShippingAddressID) ?? new Address();
                }
                return result ?? new Address();
            }
        }

        public Address BillingAddress
        {
            get
            {
                Address result = null;
                if (AddressList != null && AddressList.Any() && Order != null)
                {
                    result = AddressList.FirstOrDefault(i => i.ID == Order.BillingAddressID) ?? new Address();
                }
                return result ?? new Address();
            }
        }

        public bool IsValid(bool checkPayment = false)
        {
            var result = Order != null && Basket != null && Basket.Items!=null && Basket.Items.Any();
            if (result && checkPayment)
            {
                result = (Order.Status == OrderStatuses.WaitingCustomerApproval || Order.Status == OrderStatuses.WaitingPayment);
            }
            return result;
        }
    }
}