using System;
using System.Text.RegularExpressions;
using Aware.ECommerce.Util;
using Aware.Util.Model;

namespace Aware.Payment.Model
{
    public class CreditCard
    {
        private string _cardNumber;
        public string CardNumber
        {
            get
            {
                return _cardNumber;
            }
            set
            {
                _cardNumber = !string.IsNullOrEmpty(value) ? value.Trim().Replace(" ", "") : string.Empty;
            }
        }

        public string CardHolder { get; set; }
        public int ExpireMonth { get; set; }
        public int ExpireYear { get; set; }
        public string CVC { get; set; }

        public Result IsValid()
        {
            var message = string.Empty;
            var now = DateTime.Now;
            var regex = new Regex(Constants.CreditCardRegex);

            if (string.IsNullOrEmpty(CardNumber) || !regex.IsMatch(CardNumber))
            {
                message = Resource.Card_InvalidCardNumber;
            }
            else if (string.IsNullOrEmpty(CVC) || CVC.Length < 3 || CVC.Length > 4)
            {
                message = Resource.Card_InvalidCardNumber;
            }
            else if (ExpireYear < now.Year || ExpireYear > now.Year + 100)
            {
                message = Resource.Card_InvalidExpireDate;
            }
            else if (ExpireMonth <= 0 || ExpireMonth > 12)
            {
                message = Resource.Card_InvalidExpireDate;
            }
            return string.IsNullOrEmpty(message) ? Result.Success() : Result.Error(message);
        }
    }
}