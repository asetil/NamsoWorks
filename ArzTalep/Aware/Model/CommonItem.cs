namespace Aware.Model
{
    public class CommonItem
    {
        public virtual int ID { get; set; }

        public virtual string Title { get; set; }

        public virtual string Value { get; set; }

        public CommonItem()
        {

        }

        public CommonItem(int id, string value)
        {
            ID = id;
            Value = value;
        }

        public CommonItem(int id, string title, string value)
        {
            ID = id;
            Title = title;
            Value = value;
        }

        public CommonItem(int id, string title, string value, string url)
        {
            ID = id;
            Title = title;
            Value = value;
        }
    }
}