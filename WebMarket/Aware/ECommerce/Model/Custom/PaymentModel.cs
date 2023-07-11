namespace Aware.ECommerce.Model
{
    public class PaymentModel
    {
        public string CardHolder { get; set; }
        public string CardNumber { get; set; }
        public string CVV { get; set; }
        public string ExpireMonth { get; set; }
        public string ExpireYear { get; set; }
    }
}
