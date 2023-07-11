namespace Aware.ECommerce.Model
{
    public class StoreStatisticModel
    {
        public int StoreID { get; set; }
        public int NoStock { get; set; }
        public int RedStock { get; set; }
        public int YellowStock { get; set; }
        public int GreenStock { get; set; }
        public int InfiniteStock { get; set; }
        public long ItemCount { get; set; }
    }
}