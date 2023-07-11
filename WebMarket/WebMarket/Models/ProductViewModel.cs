using System.Collections.Generic;
using System.Linq;
using Aware.ECommerce.Model;
using Aware.Util;
using Aware.Util.Model;
using Aware.ECommerce.Enums;

namespace WebMarket.Models
{
    public class ProductViewModel
    {
        public Product Product { get; set; }
        public List<int> FavoriteProducts { get; set; }
        public bool DisplayComments { get; set; }
        public bool ShowInstallments { get; set; }
        public bool AllowCompare { get; set; }
        public bool AllowSoialShare { get; set; }
        public List<Item> PropertyList { get; private set; }

        public void ArrangeProperties(List<PropertyValue> propertyData)
        {
            var result = new List<Item>();
            if (propertyData != null && propertyData.Any() && Product != null)
            {
                var parentIDs = Product.Properties.Where(i => i.Parent > 0).Select(i => i.Parent).Distinct();
                foreach (var parentID in parentIDs)
                {
                    var parent = propertyData.FirstOrDefault(i => i.ID == parentID);
                    result.Add(GetItem(parentID, parent.Name, string.Empty, true));

                    var childs = Product.Properties.Where(i => i.Parent == parentID).OrderBy(i => i.Sort);
                    foreach (var child in childs)
                    {
                        var property = propertyData.FirstOrDefault(i => i.ID == child.ID);
                        if (property != null)
                        {
                            result.Add(GetItem(property.ID, property.Name, GetPropertyValue(propertyData, property, child.Value)));
                        }
                    }
                }

                var childsWithoutParent = Product.Properties.Where(i => i.Parent == 0).OrderBy(i => i.Sort);
                if (parentIDs.Any() && childsWithoutParent.Any())
                {
                    result.Add(GetItem(0, Resources.Resource.Other, string.Empty, true));
                }

                foreach (var child in childsWithoutParent)
                {
                    var property = propertyData.FirstOrDefault(i => i.ID == child.ID);
                    if (property != null)
                    {
                        result.Add(GetItem(property.ID, property.Name, GetPropertyValue(propertyData, property, child.Value)));
                    }
                }
            }
            PropertyList = result;
        }

        private string GetPropertyValue(List<PropertyValue> propertyData, PropertyValue property, string value)
        {
            if (property.Type == PropertyType.Selection)
            {
                var propertyOption = propertyData.FirstOrDefault(i => i.ID == value.Int());
                return propertyOption.Name;
            }

            if (property.Type == PropertyType.YesNo)
            {
                return value == "1" ? "Var" : "Yok";
            }
            return value;
        }

        private Item GetItem(int id, string title, string value, bool isParent = false)
        {
            var result = new Item(id, title, value);
            result.OK = isParent;
            return result;
        }

        public bool IsFavorite(int productID)
        {
            return productID > 0 && FavoriteProducts != null && FavoriteProducts.Contains(productID);
        }
    }
}