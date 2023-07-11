using System.Collections.Generic;
using Aware.Util.Model;

namespace Aware.Util.View
{
    public class SelectField : Field
    {
        public string BlankOption { get; set; }
        public List<Item> DataSource { get; set; }
        public bool IsMulti { get; set; }

        public bool IsSelected(int id)
        {
            return !string.IsNullOrEmpty(Value) && (Value == id.ToString() || Value.IndexOf(id.ToString().S()) > -1);
        }
    }
}
