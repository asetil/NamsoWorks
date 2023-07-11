using System.Collections.Generic;
using System.Linq;
using Aware.Util;
using Aware.ECommerce.Enums;
using Aware.Util.Enums;
using Aware.Util.Model;

namespace Aware.ECommerce.Model.Custom
{
    public class ProductRelationsModel
    {
        public int ProductID { get; set; }
        public List<PropertyValue> PropertyList { get; set; }
        public List<PropertyValue> SelectionList { get; set; }
        public List<PropertyRelation> RelationList { get; set; }
        public List<Item> HasNoOptionList { get; set; }
        public ViewTypes ViewMode { get; set; }

        public bool HasProperty
        {
            get { return PropertyList != null && PropertyList.Any(); }
        }

        public PropertyRelation GetRelation(int propertyID)
        {
            PropertyRelation relation = null;
            if (RelationList != null && RelationList.Any())
            {
                relation = RelationList.FirstOrDefault(i => i.PropertyValueID == propertyID);
            }
            return relation ?? new PropertyRelation();
        }

        public string GetRelationValue(PropertyValue property, string value)
        {
            if (property != null)
            {
                if (property.Type == PropertyType.Selection)
                {
                    var selection = SelectionList.FirstOrDefault(i => i.ParentID == property.ID && i.ID == value.Int());
                    return selection != null ? selection.Name : string.Empty;
                }
                return value;
            }
            return string.Empty;
        }
    }
}
