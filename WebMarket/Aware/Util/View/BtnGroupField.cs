using System.Collections.Generic;
using Aware.Util.Model;

namespace Aware.Util.View
{
    public class BtnGroupField : Field
    {
        public List<Item> DataSource { get; set; }
        public bool IsMulti { get; set; }
        public bool IsBool { get; set; }

        public void SetAsBool()
        {
            IsBool = true;
        }
    }
}