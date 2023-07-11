using System;

namespace Worchart.BL.Model
{
    public class Item
    {
        public virtual int ID { get; set; }
        public virtual string Title { get; set; }
        public virtual string Value { get; set; }
        public virtual string Url { get; set; }
        public virtual string Extra { get; set; }
        public virtual bool OK { get; set; }
        public virtual int Count { get; set; }

        public virtual long LID //Mysql long fix
        {
            get { return ID; }
            set {  ID = Convert.ToInt32(value); }
        }

        public static Item New(int id, string value)
        {
            return new Item(id, value);
        }

        public Item()
        {

        }

        public Item(int id, string value)
        {
            ID = id;
            Value = value;
        }

        public Item(int id, string title, string value)
        {
            ID = id;
            Title = title;
            Value = value;
        }

        public Item(int id, string title, string value, string url)
        {
            ID = id;
            Title = title;
            Value = value;
            Url = url;
        }
    }
}