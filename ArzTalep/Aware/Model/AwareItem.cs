using System;

namespace Aware.Model
{
    public class AwareItem
    {
        public int ID { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        public AwareItem(string key, string value)
        {
            Key = key;
            Value = value;

            Int32.TryParse(key, out int id);
            ID = id;
        }

        public AwareItem(int id, string value)
        {
            ID = id;
            Key = id.ToString();
            Value = value;
        }
    }
}