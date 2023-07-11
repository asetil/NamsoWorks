using System.Collections.Generic;

namespace Aware.Util.Model
{
    public class Slider
    {
        public string ID { get; set; }
        public string Css { get; set; }
        public int SwitchTime { get; set; }
        public List<Item> Items { get; set; }
        public Slider(string css="",int switchTime=3000,string id="")
        {
            Css = css;
            SwitchTime = switchTime;
            ID = id;
            Items=new List<Item>();
        }

        public void AddItem(int id, string html)
        {
           Items.Add(new Item(id,html));
        }
    }
}
