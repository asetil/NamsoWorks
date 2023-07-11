namespace Aware.Util.Model
{
    public class Pricer
    {
        public decimal Price { get; private set; }
        public string Left { get; private set; }
        public string Right { get; private set; }
        public string Currency { get; private set; }

        public Pricer(decimal price)
        {
            Price = price;
            Eval();
        }

        private void Eval()
        {
            var priceValues = Price.Split();
            Left = priceValues[0];
            Right = priceValues[1];
        }
    }
}