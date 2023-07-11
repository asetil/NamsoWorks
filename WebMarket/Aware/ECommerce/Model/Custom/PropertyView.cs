using System.Collections.Generic;
using Aware.Util;
using Aware.ECommerce.Enums;

namespace Aware.ECommerce.Model
{
    public class PropertyView
    {
        public int ID { get; set; } //PV ID
        public int Parent { get; set; } //PV ID
        public string Value { get; set; } //PR.Value (selection ise karşılık gelen değer)
        public PropertyType Type { get; set; } //PR.SortOrder
        public string Sort { get; set; } //PR.SortOrder

        public static IEnumerable<PropertyView> GetProperties(string modelAsJson)
        {
            //modelJson must be like [{"ID":1,"Name":"Description","Value":"Amasya elması","SortOrder":"1"},{"ID":2,"Name":"Beden","Value":"M","sortOrder":"3"}]
            var result = modelAsJson.DeSerialize<IEnumerable<PropertyView>>();
            if (result == null)
            {
                result = new List<PropertyView>();
            }
            return result;
        }
    }
}
